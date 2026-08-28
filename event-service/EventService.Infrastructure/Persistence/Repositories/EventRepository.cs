using EventService.Application.Common.Interfaces;
using EventService.Domain.Entities;
using EventService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EventService.Infrastructure.Persistence.Repositories;

public sealed class EventRepository : IEventRepository
{
    private readonly EventDbContext _dbContext;

    public EventRepository(EventDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IDbTransactionScope> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        return new EfDbTransactionScope(transaction);
    }

    public async Task AddAsync(Event @event, CancellationToken cancellationToken)
    {
        await _dbContext.Events.AddAsync(@event, cancellationToken);
    }

    public async Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Events
            .Include(e => e.Zones)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Events
            .Include(e => e.Zones)
            .AsNoTracking()
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    private sealed class EfDbTransactionScope : IDbTransactionScope
    {
        private readonly IDbContextTransaction _transaction;

        public EfDbTransactionScope(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public Task CommitAsync(CancellationToken cancellationToken) =>
            _transaction.CommitAsync(cancellationToken);

        public Task RollbackAsync(CancellationToken cancellationToken) =>
            _transaction.RollbackAsync(cancellationToken);

        public ValueTask DisposeAsync() => _transaction.DisposeAsync();
    }
}
