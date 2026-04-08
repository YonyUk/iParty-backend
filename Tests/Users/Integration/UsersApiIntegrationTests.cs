using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Users.Application.DTOs;
using Users.Domain;

namespace Tests.Integration.Users;

public class UsersApiIntegrationTests : UsersBaseIntegrationTests
{
    public UsersApiIntegrationTests(CustomWebApplicationFactory applicationFactory) : base(applicationFactory)
    {
        
    }

    [Fact]
    public async Task TestCreateUser()
    {
        var data = new RegisterUserDTO("yonyuk","user@gmail.com","yony01uk",UserRole.User);

        var response = await client.PostAsJsonAsync("/api/v1/users/register",data);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}