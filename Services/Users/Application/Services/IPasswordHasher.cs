namespace Users.Application.Services;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string plainPassword, string hash);
}