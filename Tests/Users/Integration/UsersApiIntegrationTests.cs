using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Renci.SshNet.Sftp;
using Users.API.Converters;
using Users.Application.DTOs;
using Users.Domain;

namespace Tests.Integration.Users;

public enum TestCreateUserExpectedResult
{
    Ok = 0,
    BadRequest,
    Conflict
}
public enum TestLoginExpectedResult
{
    Ok = 0,
    BadRequest,
    UserNotFound,
    Unauthorized
}
public class UsersApiIntegrationTests : UsersBaseIntegrationTests
{
    private readonly JsonSerializerOptions enumsSerializerOptions;
    public UsersApiIntegrationTests(CustomWebApplicationFactory applicationFactory) : base(applicationFactory)
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new UserRoleJsonConverter());
        enumsSerializerOptions = options;
    }

    async Task<string> LoginUser(string username,UserRole role)
    {
        await CreateUser(username,role);
        var formData = new Dictionary<string, string>
        {
            {"username",username},
            {"password",$"{username}@password"}
        };
        using var content = new FormUrlEncodedContent(formData);
        var response = await client.PostAsync("/api/users/login",content);
        response.Headers.TryGetValues("Set-Cookie",out var cookies);
        var token = cookies!.FirstOrDefault(c => c.StartsWith("access_token="));
        return token!.Split(";").First();
    }
    async Task CreateUser(string username, UserRole role,string? password = null)
    {
        var formData = new Dictionary<string, string>
        {
            {"username",username},
            {"email",$"{username}@gmail.com"},
            {"password",password ?? $"{username}@password"},
            {"role",role.ToString()}
        };
        using var content = new FormUrlEncodedContent(formData);
        await client.PostAsync("/api/users/register", content);
    }
    async Task Populate(params string[] usernames)
    {
        for (int i = 0; i < usernames.Length; i++)
            await CreateUser(usernames[i], (UserRole)(i & 1));
    }

    [Theory]
    [InlineData("yonyuk", "user@gmail.com", "yony01uk", UserRole.User, TestCreateUserExpectedResult.Ok)]
    [InlineData("yonyuk", "user@gmail.com", "yony01uk", UserRole.Host, TestCreateUserExpectedResult.Ok)]
    [InlineData("yony", "user@gmail.com", "yony01uk", UserRole.User, TestCreateUserExpectedResult.BadRequest)]
    [InlineData("yonyuk", "usergmail.com", "yony01uk", UserRole.User, TestCreateUserExpectedResult.BadRequest)]
    [InlineData("yonyuk", "user@gmail.com", "yony", UserRole.User, TestCreateUserExpectedResult.BadRequest)]
    [InlineData(null, "user@gmail.com", "yony01uk", UserRole.User, TestCreateUserExpectedResult.BadRequest)]
    [InlineData("yonyuk", null, "yony01uk", UserRole.User, TestCreateUserExpectedResult.BadRequest)]
    [InlineData("yonyuk", "user@gmail.com", null, UserRole.User, TestCreateUserExpectedResult.BadRequest)]
    [InlineData("   ", "user@gmail.com", "yony01uk", UserRole.User, TestCreateUserExpectedResult.BadRequest)]
    [InlineData("yonyuk", "   ", "yony01uk", UserRole.User, TestCreateUserExpectedResult.BadRequest)]
    [InlineData("yonyuk", "user@gmail.com", "   ", UserRole.User, TestCreateUserExpectedResult.BadRequest)]
    [InlineData("yonyuk", "user@gmail.com", "yony01uk", UserRole.User, TestCreateUserExpectedResult.Conflict)]
    public async Task TestCreateUser(string? username, string? email, string? password, UserRole role, TestCreateUserExpectedResult expected)
    {
        var formData = new Dictionary<string, string>
        {
            {"username",username},
            {"email",email},
            {"password",password},
            {"role",role == UserRole.User ? "user" : "host"}
        };
        using var content = new FormUrlEncodedContent(formData);
        var response = await client.PostAsync("/api/users/register", content);

        if (expected == TestCreateUserExpectedResult.Conflict)
            response = await client.PostAsync("api/users/register", content);

        switch (expected)
        {
            case TestCreateUserExpectedResult.BadRequest:
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                break;

            case TestCreateUserExpectedResult.Conflict:
                response.StatusCode.Should().Be(HttpStatusCode.Conflict);
                break;

            default:
                response.StatusCode.Should().Be(HttpStatusCode.Created);
                break;
        }
    }

    [Theory]
    [InlineData("yonyuk", "yony01uk", TestLoginExpectedResult.Ok)]
    [InlineData("yony", "yony01uk", TestLoginExpectedResult.BadRequest)]
    [InlineData("yonyuk", "yony", TestLoginExpectedResult.BadRequest)]
    [InlineData(null, "yony01uk", TestLoginExpectedResult.BadRequest)]
    [InlineData("yonyuk", null, TestLoginExpectedResult.BadRequest)]
    [InlineData("   ", "yony01uk", TestLoginExpectedResult.BadRequest)]
    [InlineData("yonyuk", "   ", TestLoginExpectedResult.BadRequest)]
    [InlineData("yonyuk", "yony01uk", TestLoginExpectedResult.UserNotFound)]
    [InlineData("yonyuk", "yony02uk", TestLoginExpectedResult.Unauthorized)]
    public async Task TestLoginUser(string? username, string? password, TestLoginExpectedResult expected)
    {
        var formData = new Dictionary<string, string>{
            {"username",username},
            {"password",password}
        };

        if (expected != TestLoginExpectedResult.UserNotFound)
            await CreateUser("yonyuk",UserRole.User,"yony01uk");

        using var content = new FormUrlEncodedContent(formData);
        var response = await client.PostAsync("/api/users/login", content);

        switch (expected)
        {
            case TestLoginExpectedResult.UserNotFound:
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
                break;

            case TestLoginExpectedResult.BadRequest:
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                break;

            case TestLoginExpectedResult.Unauthorized:
                response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
                break;

            default:
                response.StatusCode.Should().Be(HttpStatusCode.Accepted);
                break;
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TestGetUserById(bool exists)
    {
        await Populate("yonyuk");
        var response = await client.GetAsync("/api/users");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<UserDTO[]>(enumsSerializerOptions);
        var userId = users![0].Id;

        response = await client.GetAsync($"/api/users/{(exists ? userId.ToString() : Guid.NewGuid().ToString())}");

        if (exists)
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var user = await response.Content.ReadFromJsonAsync<UserDTO>(enumsSerializerOptions);
            user.Should().NotBeNull();
            user.Id.Should().Be(userId);
        }
        else
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(null,0)]
    [InlineData(null,1)]
    [InlineData(UserRole.User,0)]
    [InlineData(UserRole.User,1)]
    [InlineData(UserRole.Host,0)]
    [InlineData(UserRole.Host,1)]
    public async Task TestGetUsers(UserRole? role,int page)
    {
        await Populate("yonyuk","jose01","brayan","nayeli","lauren","alexander");
        var response = await client.GetAsync($"/api/users?page={page}{(role != null ? $"&role={role}" : "")}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<UserDTO[]>(enumsSerializerOptions);
        if (role != null)
            users.Should().OnlyContain(user => user.Role == role);
        if (page != 0)
            users!.Length.Should().Be(0);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TestGetUserByName(bool exists)
    {
        if (exists)
            await CreateUser("yonyuk",UserRole.User);

        var response = await client.GetAsync($"/api/users/name/yonyuk");

        if (exists)
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var user = await response.Content.ReadFromJsonAsync<UserDTO>(enumsSerializerOptions);
            user!.UserName.Should().Be("yonyuk");
        }
        else
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TestGetUserByEmail(bool exists)
    {
        if (exists)
            await CreateUser("yonyuk",UserRole.User);
        
        var response = await client.GetAsync("/api/users/email/yonyuk@gmail.com");

        if (exists)
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var user = await response.Content.ReadFromJsonAsync<UserDTO>(enumsSerializerOptions);
            user!.Email.Should().Be("yonyuk@gmail.com");
        }
        else
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(true,UserRole.User)]
    [InlineData(true,UserRole.Host)]
    [InlineData(false,UserRole.User)]
    [InlineData(false,UserRole.Host)]
    public async Task TestGetCurrentUser(bool logged, UserRole role)
    {
        if (logged)
        {
            var token = await LoginUser("yonyuk",role);
            client.DefaultRequestHeaders.Add("Cookie",token);
        }
        
        var response = await client.GetAsync("/api/users/me");

        if (logged)
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var user = await response.Content.ReadFromJsonAsync<UserDTO>(enumsSerializerOptions);
            user!.UserName.Should().Be("yonyuk");
            user!.Email.Should().Be("yonyuk@gmail.com");
            user!.Role.Should().Be(role);
        }
        else
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}