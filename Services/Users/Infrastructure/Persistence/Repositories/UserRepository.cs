using Microsoft.EntityFrameworkCore;
using Npgsql;
using Users.Domain;
using Users.Domain.Aggregates;
using Users.Domain.Exceptions;
using Users.Domain.ValueObjects;

namespace Users.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext appDbContext;
    public UserRepository(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }
    public async Task<Guid> Create(User user, CancellationToken token = default)
    {
        try
        {
            await appDbContext.Users.AddAsync(user, token);
            return user.Id;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException postgresException && postgresException.SqlState == "23505")
        {
            if (postgresException.ConstraintName?.Contains("UserName") == true)
                throw new UserAlreadyExistsException(nameof(UserName), user.UserName.Value);
            if (postgresException.ConstraintName?.Contains("Email") == true)
                throw new UserAlreadyExistsException(nameof(Email), user.Email.Value);
            throw;
        }
    }

    public async Task Delete(Guid id, CancellationToken token = default)
    {
        var user = await appDbContext.Users.FindAsync(id, token);
        if (user == null)
            throw new UserNotFoundException(nameof(User.Id), id.ToString());
        appDbContext.Users.Remove(user);
    }

    public async Task<User> GetByEmail(Email email, CancellationToken token = default)
    {
        var user = await appDbContext.Users.SingleOrDefaultAsync(user => user.Email.Value == email.Value, token);
        if (user == null)
            throw new UserNotFoundException(nameof(User.Email), email.Value);
        return user;
    }

    public async Task<User> GetById(Guid id, CancellationToken token = default)
    {
        var user = await appDbContext.Users.FindAsync(id, token);
        if (user == null)
            throw new UserNotFoundException(nameof(User.Id), id.ToString());
        return user;
    }

    public async Task<User> GetByName(UserName username, CancellationToken token = default)
    {
        var user = await appDbContext.Users.SingleOrDefaultAsync(user => user.UserName.Value == username.Value);
        if (user == null)
            throw new UserNotFoundException(nameof(User.UserName), username.Value);
        return user;
    }

    public async Task<IEnumerable<User>> GetUsers(CancellationToken token = default)
    {
        var users = await appDbContext.Users.AsNoTracking().ToListAsync();
        return users;
    }

    public async Task<IEnumerable<User>> GetUsersByRole(UserRole role, CancellationToken token = default)
    {
        var users = await appDbContext.Users.Where(user => user.Role == role).AsNoTracking().ToListAsync();
        return users;
    }
}