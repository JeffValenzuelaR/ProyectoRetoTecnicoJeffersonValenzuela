using EventService.Domain.Entities;

namespace EventService.Application.Common.Interfaces;

public interface IEventRepository
{

    Task<IDbTransactionScope> BeginTransactionAsync(CancellationToken cancellationToken);

    Task AddAsync(Event @event, CancellationToken cancellationToken);

    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Event>> GetAllAsync(CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IDbTransactionScope : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken);

    Task RollbackAsync(CancellationToken cancellationToken);
}
