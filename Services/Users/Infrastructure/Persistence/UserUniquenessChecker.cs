using Microsoft.EntityFrameworkCore;
using Users.Domain.Services;
using Users.Domain.ValueObjects;

namespace Users.Infrastructure.Persistence;

public class UserUniquenessChecker : IUserUniquenessChecker
{
    private readonly AppDbContext appDbContext;
    public UserUniquenessChecker(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }
    public async Task<bool> IsUnique(UserName username) => !await appDbContext.Users.AnyAsync(user => user.UserName.Value == username.Value);

    public async Task<bool> IsUnique(Email email) => !await appDbContext.Users.AnyAsync(user => user.Email.Value == email.Value);
}