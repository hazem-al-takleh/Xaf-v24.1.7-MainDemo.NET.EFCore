using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using Microsoft.EntityFrameworkCore;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using DevExpress.ExpressApp.ApplicationBuilder;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using MainDemo.MiddleTier.JWT;
using DevExpress.ExpressApp.Security.Authentication.ClientServer;
using DevExpress.ExpressApp;
using Demos.Data;
using MainDemo.Module;
using MainDemo.Module.BusinessObjects;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Notifications;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.Office;
using DevExpress.ExpressApp.ViewVariantsModule;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.Negotiate;
namespace MainDemo.MiddleTier;

public class Startup {
    public Startup(IConfiguration configuration) {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services) {
        services.AddScoped<IAuthenticationTokenProvider, JwtTokenProviderService>();

        services.AddXafMiddleTier(Configuration, builder => {
            builder.Modules
            .AddAuditTrailEFCore()
            .Add<ConditionalAppearanceModule>()
            .Add<NotificationsModule>()
            .Add<OfficeModule>()
            .Add<PivotChartModuleBase>()
            .AddReports(options => {
                options.EnableInplaceReports = true;
                options.ReportDataType = typeof(DevExpress.Persistent.BaseImpl.EF.ReportDataV2);
                options.ReportStoreMode = DevExpress.ExpressApp.ReportsV2.ReportStoreModes.XML;
            })
            .AddValidation(options => {
                options.AllowValidationDetailsAccess = false;
            })
            .Add<ViewVariantsModule>()
            .AddMainDemoModule();

            builder.ObjectSpaceProviders
                .AddSecuredEFCore()
                .WithAuditedDbContext(contexts => {
                    string connectionString = null;
                    if(Configuration.GetConnectionString("ConnectionString") != null) {
                        connectionString = Configuration.GetConnectionString("ConnectionString");
                    }
                    bool isSqlServerAccessible = DemoDbEngineDetectorHelper.IsSqlServerAccessible();
                    contexts.Configure<MainDemoDbContext, AuditingDbContext>(
                        (application, businessObjectDbContextOptions) => {
                            if(isSqlServerAccessible) {
                                businessObjectDbContextOptions.UseSqlServer(connectionString);
                            }
                            else {
                                businessObjectDbContextOptions.UseInMemoryDatabase("InMemory");
                            }
                            businessObjectDbContextOptions.UseChangeTrackingProxies();
                            businessObjectDbContextOptions.UseObjectSpaceLinkProxies();
                            businessObjectDbContextOptions.ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.MultipleCollectionIncludeWarning));
                        },
                        (application, auditHistoryDbContextOptions) => {
                            if(isSqlServerAccessible) {
                                auditHistoryDbContextOptions.UseSqlServer(connectionString);
                            }
                            else {
                                auditHistoryDbContextOptions.UseInMemoryDatabase("InMemory");
                            }
                            auditHistoryDbContextOptions.UseChangeTrackingProxies();
                        });
                })
            .AddNonPersistent();

            builder.Security
                .UseIntegratedMode(options => {
                    // See the security configuration in the MainDemoModuleExtensions.ConfigureSecurity method.
                })
                .AddPasswordAuthentication(options => {
                    options.IsSupportChangePassword = true;
                })
                .AddWindowsAuthentication(options => {
                    options.CreateUserAutomatically((objectSpace, user) => {
                        ((ApplicationUser)user).Roles.Add(objectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == "Users"));
                    });
                })
                .AddAuthenticationProvider<CustomAuthenticationProvider>();

            builder.AddBuildStep(application => {
                application.ApplicationName = "MainDemo";
                application.CheckCompatibilityType = DevExpress.ExpressApp.CheckCompatibilityType.DatabaseSchema;
                #if DEBUG
                //if(System.Diagnostics.Debugger.IsAttached) {
                application.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
                //}
                #endif
                application.DatabaseVersionMismatch += (s, e) => {
                    e.Updater.Update();
                    e.Handled = true;
                };
            });
        });

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddNegotiate(NegotiateDefaults.AuthenticationScheme, "Windows", opt => { })
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters() {
                    ValidateIssuerSigningKey = true,
                    //ValidIssuer = Configuration["Authentication:Jwt:Issuer"],
                    //ValidAudience = Configuration["Authentication:Jwt:Audience"],
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:Jwt:IssuerSigningKey"]))
                };
            });
            /* .AddMicrosoftIdentityWebApi(
                jwtBearerOptions => {
                    jwtBearerOptions.TokenValidationParameters.NameClaimType = "preferred_username";
                },
                msIdentityOptions => {
                    Configuration.Bind("Authentication:AzureAd", msIdentityOptions);
                },
                jwtBearerScheme: "AzureAd"); */

        services.AddAuthorization(options => {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(
                JwtBearerDefaults.AuthenticationScheme,
                "AzureAd",
                NegotiateDefaults.AuthenticationScheme
                )
                    .RequireAuthenticatedUser()
                    .RequireXafAuthentication()
                    .Build();
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime hostApplicationLifetime) {
        if(env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
        }
        else {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. To change this for production scenarios, see: https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseRequestLocalization();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseXafMiddleTier();
        app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
        });
    }
}
