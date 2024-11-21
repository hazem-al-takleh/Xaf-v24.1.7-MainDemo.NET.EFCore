using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security;
using MainDemo.Blazor.Server;
using MainDemo.Module.BusinessObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace MainDemo.WebAPI.TestInfrastructure {
    //Shared Context between Tests https://xunit.net/docs/shared-context
    [CollectionDefinition("SharedTestHost")]
    public class TestHostCollection : ICollectionFixture<SharedTestHostHolder> {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    public class SharedTestHostHolder {
        private readonly IHost host;

        public SharedTestHostHolder() {
            host = SetupTestHost();
        }

        public IHost Host => host;

        private IHost SetupTestHost() {
            var server = new HostBuilder()
            .ConfigureAppConfiguration((context, config) => {
                config.Sources.Clear();
                config.AddJsonFile("appsettings.json"); // linked from Blazor.Server
                config.AddEnvironmentVariables();
            })
            .ConfigureWebHost(webBuilder => {
                ConfigureWebHost(webBuilder);
                webBuilder.UseTestServer().UseStartup<MainDemo.Blazor.Server.Startup>();
            }).Start();
            return server;
        }

        private IWebHostBuilder ConfigureWebHost(IWebHostBuilder hostBuilder) {
            hostBuilder.ConfigureServices(s => {
                s.PostConfigure<SecurityOptions>(options => {
                    options.Events.OnCustomizeSecurityCriteriaOperator += context => {
                        DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(context.ServiceProvider, nameof(context.ServiceProvider));
                        var security = context.ServiceProvider.GetRequiredService<ISecurityStrategyBase>();
                        DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(security, nameof(ISecurityStrategyBase));
                        if(context.Operator is FunctionOperator functionOperator) {
                            if(functionOperator.Operands.Count == 1 &&
                            "MyCustomCurrentUserId".Equals((functionOperator.Operands[0] as ConstantValue)?.Value?.ToString(), StringComparison.InvariantCultureIgnoreCase)) {
                                context.Result = new ConstantValue(((ApplicationUser)context.Security.User)?.ID ?? Guid.NewGuid());
                            }
                        }
                    };
                });
            });

            return hostBuilder;
        }

        public void Dispose() => host?.Dispose();
    }
}
