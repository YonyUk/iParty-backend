using Common.Domain;
using MediatR;

namespace Users.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext appDbContext;
    private readonly IMediator mediator;
    public UnitOfWork(AppDbContext appDbContext,IMediator mediator)
    {
        this.appDbContext = appDbContext;
        this.mediator = mediator;
    }
    public async Task Commit(CancellationToken token = default)
    {
        var aggregates = appDbContext.ChangeTracker.Entries<AggregateRoot<Guid>>()
            .Where(e => e.Entity.Events.Any())
            .Select(e => e.Entity)
            .ToList();
        
        var events = aggregates.SelectMany(aggregate => aggregate.Events).ToList();
        // creates the transaction
        await using var transaction = await appDbContext.Database.BeginTransactionAsync(token);

        try
        {
            await appDbContext.SaveChangesAsync(token);

            foreach(var domainEvent in events)
            {
                var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
                var notification = Activator.CreateInstance(notificationType,domainEvent);
                await mediator.Publish(notification,token);
            }
            
            await transaction.CommitAsync(token);

            foreach(var aggregate in aggregates)
                aggregate.ClearEvents();
        }
        catch (Exception)
        {
            
            throw;
        }
    }
}