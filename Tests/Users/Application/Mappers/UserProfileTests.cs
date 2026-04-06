using AutoMapper;
using Users.Application.Mappers;

namespace Tests.Unit.Users.Application.Mappers;

public class UserProfileTests
{
    [Fact]
    public void TestUserProfile()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>());
        config.AssertConfigurationIsValid();
    }
}