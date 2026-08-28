namespace EventService.Application.Common.Interfaces;

public interface IEventCreatedPublisher
{
    Task PublishEventCreatedAsync(Guid eventId, string name, CancellationToken cancellationToken);
}
