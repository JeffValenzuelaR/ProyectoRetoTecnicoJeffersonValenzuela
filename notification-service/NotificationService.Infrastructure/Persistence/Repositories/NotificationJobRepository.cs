using MongoDB.Driver;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Persistence.Repositories;

public sealed class NotificationJobRepository : INotificationJobRepository
{
    private readonly IMongoCollection<NotificationJob> _collection;

    public NotificationJobRepository(MongoDbContext context)
    {
        _collection = context.NotificationJobs;
    }

    public async Task<IReadOnlyList<NotificationJob>> GetLatestAsync(int take = 50, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(FilterDefinition<NotificationJob>.Empty)
            .SortByDescending(job => job.CreatedAt)
            .Limit(take)
            .ToListAsync(cancellationToken);
    }

    public Task<NotificationJob?> FindByMessageIdAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        return _collection
            .Find(job => job.MessageId == messageId)
            .FirstOrDefaultAsync(cancellationToken)!;
    }

    public async Task<bool> TryInsertAsync(NotificationJob job, CancellationToken cancellationToken = default)
    {
        try
        {
            await _collection.InsertOneAsync(job, options: null, cancellationToken: cancellationToken);
            return true;
        }
        catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
        {

            return false;
        }
    }

    public Task UpdateAsync(NotificationJob job, CancellationToken cancellationToken = default)
    {
        return _collection.ReplaceOneAsync(
            existing => existing.Id == job.Id,
            job,
            options: new ReplaceOptions(),
            cancellationToken: cancellationToken);
    }
}
