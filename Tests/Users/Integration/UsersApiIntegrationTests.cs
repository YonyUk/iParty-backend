using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Renci.SshNet.Sftp;
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
    public UsersApiIntegrationTests(CustomWebApplicationFactory applicationFactory) : base(applicationFactory)
    {

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
        var data = new RegisterUserDTO(username, email, password, role);
        var response = await client.PostAsJsonAsync("/api/users/register", data);

        if (expected == TestCreateUserExpectedResult.Conflict)
            response = await client.PostAsJsonAsync("api/users/register", data);

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
    // [InlineData("yonyuk", "yony01uk", TestLoginExpectedResult.Ok)]
    // [InlineData("yony", "yony01uk", TestLoginExpectedResult.BadRequest)]
    // [InlineData("yonyuk", "yony", TestLoginExpectedResult.BadRequest)]
    // [InlineData(null, "yony01uk", TestLoginExpectedResult.BadRequest)]
    // [InlineData("yonyuk", null, TestLoginExpectedResult.BadRequest)]
    // [InlineData("   ", "yony01uk", TestLoginExpectedResult.BadRequest)]
    // [InlineData("yonyuk", "   ", TestLoginExpectedResult.BadRequest)]
    [InlineData("yonyuk", "yony01uk", TestLoginExpectedResult.UserNotFound)]
    // [InlineData("yonyuk", "yony01uk", TestLoginExpectedResult.Unauthorized)]
    public async Task TestLoginUser(string? username,string? password, TestLoginExpectedResult expected)
    {
        var formData = new Dictionary<string,string>{
            {"username",username},
            {"password",password}
        };
        using var content = new FormUrlEncodedContent(formData);
        var response = await client.PostAsync("/api/users/login",content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }
}