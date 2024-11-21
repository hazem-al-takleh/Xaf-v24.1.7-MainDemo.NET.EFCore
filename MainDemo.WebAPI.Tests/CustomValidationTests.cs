using System.Net;
using MainDemo.Module.BusinessObjects;
using MainDemo.WebAPI.TestInfrastructure;
using Xunit;

namespace MainDemo.WebAPI.Tests;

public class CustomValidationTests : BaseWebApiTest {
    public CustomValidationTests(SharedTestHostHolder fixture) : base(fixture) { }

    //The validation is not supported by default and the ApplicationUser with empty the UserName can be created
    //The CustomDataService service is implemented in this demo application
    [Fact]
    public async System.Threading.Tasks.Task CreateApplicationUser_ValidateUserNameIsNotEmpty() {
        //Use an admin user to be able to create the ApplicationUser object 
        await WebApiClient.AuthenticateAsync("Sam", "");
        var errorResult = await Assert.ThrowsAnyAsync<HttpRequestException>(() => WebApiClient.PostAsync(new ApplicationUser()));

        string expectedErrorMessage =
            $"Bad Request : Data Validation Error: Please review and correct the data validation error(s) listed below to proceed.{Environment.NewLine}" +
            $" - The user name must not be empty";
        Assert.Equal(expectedErrorMessage, errorResult.Message);
        Assert.Equal(HttpStatusCode.BadRequest, errorResult.StatusCode);


        var newUser = _ = await WebApiClient.PostAsync(new ApplicationUser() {
            UserName = "CreateApplicationUser_ValidateUserNameIsNotEmpty"
        });
        Assert.NotNull(newUser);
        await WebApiClient.DeleteAsync<ApplicationUser>(newUser.ID.ToString());
    }
}

