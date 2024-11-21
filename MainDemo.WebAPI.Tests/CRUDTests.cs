using Xunit;
using MainDemo.WebAPI.TestInfrastructure;
using System.Net;
using MainDemo.Module.BusinessObjects;
using DevExpress.ExpressApp.Security;

namespace MainDemo.WebAPI.Tests;

public class CRUDTests : BaseWebApiTest {
    public CRUDTests(SharedTestHostHolder fixture) : base(fixture) { }

    [Fact]
    public async System.Threading.Tasks.Task CheckUnauthorizedAccess() {
        try {
            await WebApiClient.GetAllAsync(typeof(Resume), true);
            Assert.True(false, "The HttpRequestException is expected : Unauthorized");
        }
        catch(HttpRequestException e) {
            Assert.Equal(HttpStatusCode.Unauthorized, e.StatusCode);
        }
    }

    [Theory]
    [InlineData(typeof(Employee))]
    [InlineData(typeof(Resume))]
    public async System.Threading.Tasks.Task GetBasic(Type boType) {
        await WebApiClient.AuthenticateAsync("John", "");
        var items = await WebApiClient.GetAllAsync(boType);
        Assert.True(items.Length > 0);
    }


    [Fact]
    public async System.Threading.Tasks.Task CreateApplicationUser() {
        //Use an admin user to be able to create the ApplicationUser object 
        await WebApiClient.AuthenticateAsync("Sam", "");
        ApplicationUser newUser = null;

        try {
            newUser = await WebApiClient.PostAsync(new ApplicationUser() {
                UserName = "<TestUser>"
            });

            Assert.NotNull(newUser);
            Assert.Equal("<TestUser>", newUser.UserName);

            var newUser_Loaded = await WebApiClient.GetByKeyAsync<ApplicationUser>(newUser.ID.ToString());
            Assert.Equal(newUser.UserName, newUser_Loaded.UserName);
            Assert.Equal(newUser.ID, newUser_Loaded.ID);
        }
        finally {
            if(newUser != null) {
                var deletedObjectKey = (await WebApiClient.DeleteAsync<ApplicationUser>(newUser.ID.ToString())).ID;
                Assert.Equal(newUser.ID, deletedObjectKey);
            }
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task GetApplicationUserWithRef() {
        await WebApiClient.AuthenticateAsync("Sam", "");

        var results = await WebApiClient.GetAllAsync<ApplicationUser>(nameof(ApplicationUser.UserLogins));
        var admin = results.First(x => x.UserName == "Sam");
        Assert.Equal(SecurityDefaults.PasswordAuthentication, admin.UserLogins[0].LoginProviderName);
    }

    [Fact] //one-to-many
    public async System.Threading.Tasks.Task Assign_an_object_to_a_reference_property() {
        await WebApiClient.AuthenticateAsync("Sam", "");

        Employee _newEmployee = null;
        try {
            _newEmployee = await WebApiClient.PostAsync(new Employee() {
                FirstName = "Test",
                LastName = "Test",
                Email = "Test@com.com",
            });

            var newEmployee = await WebApiClient.GetByKeyAsync<Employee>(_newEmployee.ID.ToString(), nameof(Employee.Department));
            Assert.NotNull(newEmployee);
            Assert.Null(newEmployee.Department);


            var departments = await WebApiClient.GetAllAsync<Department>();
            var department = departments.First();
            Assert.NotNull(department);

            await WebApiClient.CreateRefAsync<Employee, Department>(newEmployee.ID.ToString(), nameof(Employee.Department), department.ID.ToString());

            newEmployee = await WebApiClient.GetByKeyAsync<Employee>(newEmployee.ID.ToString(), nameof(Employee.Department));
            Assert.NotNull(newEmployee);
            Assert.NotNull(newEmployee.Department);
            Assert.Equal(newEmployee.Department.ID, department.ID);
        }
        finally {
            if(_newEmployee != null) {
                var deletedObjectKey = (await WebApiClient.DeleteAsync<Employee>(_newEmployee.ID.ToString())).ID;
                Assert.Equal(_newEmployee.ID, deletedObjectKey);
            }
        }
    }

    [Fact] //many-to-many
    public async System.Threading.Tasks.Task Add_an_object_to_a_collection() {
        await WebApiClient.AuthenticateAsync("Sam", "");

        DemoTask _newTask = null;
        try {
            _newTask = await WebApiClient.PostAsync(new DemoTask() {
                Subject = "123",
                Status = Module.BusinessObjects.TaskStatus.NotStarted
            });
            ;

            var newTask = await WebApiClient.GetByKeyAsync<DemoTask>(_newTask.ID.ToString(), nameof(DemoTask.Employees));
            Assert.NotNull(newTask);
            Assert.Empty(newTask.Employees);


            var employees = await WebApiClient.GetAllAsync<Employee>();
            var employee = employees.First();
            Assert.NotNull(employee);
            if(employee.Tasks != null) {
                Assert.Null(employee.Tasks.FirstOrDefault(r => r.ID == newTask.ID));
            }

            await WebApiClient.CreateRefAsync<Employee, DemoTask>(employee.ID.ToString(), nameof(Employee.Tasks), newTask.ID.ToString());

            employee = await WebApiClient.GetByKeyAsync<Employee>(employee.ID.ToString(), nameof(employee.Tasks));
            Assert.NotNull(employee);
            Assert.NotNull(employee.Tasks.FirstOrDefault(r => r.ID == newTask.ID));

            newTask = await WebApiClient.GetByKeyAsync<DemoTask>(newTask.ID.ToString(), nameof(DemoTask.Employees));
            Assert.NotNull(newTask.Employees.FirstOrDefault(r => r.ID == employee.ID));
        }
        finally {
            if(_newTask != null) {
                var deletedObjectKey = (await WebApiClient.DeleteAsync<DemoTask>(_newTask.ID.ToString())).ID;
                Assert.Equal(_newTask.ID, deletedObjectKey);
            }
        }
    }
}
