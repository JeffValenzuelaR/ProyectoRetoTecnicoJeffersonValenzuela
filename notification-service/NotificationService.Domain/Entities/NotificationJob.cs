using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Entities;

public sealed class NotificationJob
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid MessageId { get; set; }

    public Guid EventId { get; set; }

    public string EventName { get; set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; set; }

    public Guid CorrelationId { get; set; }

    public string PayloadHash { get; set; } = string.Empty;

    public NotificationStatus Status { get; set; } = NotificationStatus.Processing;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public string? ErrorMessage { get; set; }
}
