using EventService.Application.Common.Interfaces;
using MassTransit;
using Shared.Contracts;

namespace EventService.Infrastructure.Messaging;

public sealed class EventCreatedPublisher : IEventCreatedPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public EventCreatedPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishEventCreatedAsync(Guid eventId, string name, CancellationToken cancellationToken)
    {
        var message = new EventCreated(
            MessageId: Guid.NewGuid(),
            EventId: eventId,
            Name: name,
            OccurredAt: DateTimeOffset.UtcNow,
            CorrelationId: Guid.NewGuid());

        return _publishEndpoint.Publish(message, cancellationToken);
    }
}
