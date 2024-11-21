using MainDemo.Module.BusinessObjects;
using MainDemo.WebAPI.TestInfrastructure;
using Xunit;

namespace MainDemo.WebAPI.Tests;

public class MediaFileTests : BaseWebApiTest {
    public MediaFileTests(SharedTestHostHolder fixture) : base(fixture) { }

    [Fact]
    public async System.Threading.Tasks.Task LoadApplicationUserPhotoTest() {
        await WebApiClient.AuthenticateAsync("Sam", "");
        var userSamId = (await WebApiClient.GetAllAsync<ApplicationUser>()).First(u => u.UserName == "Sam").ID;

        var photo = await WebApiClient.DownloadStream<ApplicationUser>(userSamId.ToString(), nameof(ApplicationUser.Photo));
        Assert.True(photo.Length > 1000);
    }
}

