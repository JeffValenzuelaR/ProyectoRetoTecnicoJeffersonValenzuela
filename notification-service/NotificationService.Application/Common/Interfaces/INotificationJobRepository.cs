using NotificationService.Domain.Entities;

namespace NotificationService.Application.Common.Interfaces;

public interface INotificationJobRepository
{
    Task<IReadOnlyList<NotificationJob>> GetLatestAsync(int take = 50, CancellationToken cancellationToken = default);

    Task<NotificationJob?> FindByMessageIdAsync(Guid messageId, CancellationToken cancellationToken = default);

    Task<bool> TryInsertAsync(NotificationJob job, CancellationToken cancellationToken = default);

    Task UpdateAsync(NotificationJob job, CancellationToken cancellationToken = default);
}
