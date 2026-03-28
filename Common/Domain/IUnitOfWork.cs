namespace Common.Domain;

/// <summary>
/// Describes an entity which makes atomic operations
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Save all the changes in an atomic operation
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task Commit(CancellationToken token = default);
}