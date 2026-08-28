namespace Shared.Contracts;

public sealed record EventCreated(
    Guid MessageId,
    Guid EventId,
    string Name,
    DateTimeOffset OccurredAt,
    Guid CorrelationId,
    int Version = 1
);
