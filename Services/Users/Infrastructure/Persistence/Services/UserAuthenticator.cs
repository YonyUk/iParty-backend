using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Users.Application.Services;
using Users.Domain.Aggregates;
using Users.Infrastructure.Configuration;

namespace Users.Infrastructure.Services.Security;

public class UserAuthenticator : IUserAuthenticator
{
    private readonly JwtConfigOptions jwtConfigOptions;
    public UserAuthenticator(IOptions<JwtConfigOptions> options) => jwtConfigOptions = options.Value;
    public string Authenticate(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Email,user.Email.Value),
            new Claim(ClaimTypes.Role,user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigOptions.SecretKey));
        var credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha512);
        var token = new JwtSecurityToken(
            issuer: jwtConfigOptions.Issuer,
            audience: jwtConfigOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtConfigOptions.ExpiresMinutes),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}