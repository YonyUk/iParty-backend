using Users.Domain.Aggregates;

namespace Users.Application.Services;

public interface IUserAuthenticator
{
    /// <summary>
    /// Authenticates an user
    /// </summary>
    /// <param name="user"></param>
    /// <returns>A json-web-token with user's credentials</returns>
    string Authenticate(User user);
}