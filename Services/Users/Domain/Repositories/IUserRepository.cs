using Users.Domain.Aggregates;
using Users.Domain.ValueObjects;

namespace Users.Domain;

public interface IUserRepository
{
    Task<Guid> Create(User user, CancellationToken token = default);
    Task<User> GetById(Guid id,CancellationToken token = default);
    Task<User> GetByName(UserName username,CancellationToken token = default);
    Task<User> GetByEmail(Email email,CancellationToken token = default);
    Task<IEnumerable<User>> GetUsers();
    Task<IEnumerable<User>> GetUsersByRole(UserRole role);
    Task Delete(Guid id,CancellationToken token = default);
}