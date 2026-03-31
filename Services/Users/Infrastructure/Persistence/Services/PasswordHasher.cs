using BCrypt.Net;
using Users.Application.Services;

namespace Users.Infrastructure.Services.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int workFactor = 12;
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password,workFactor);

    public bool Verify(string plainPassword, string hash) => BCrypt.Net.BCrypt.Verify(plainPassword,hash);
}