using EventService.Application.Common.Dtos;

namespace EventService.Application.Common.Interfaces;

public interface IEventCacheService
{
    Task<IReadOnlyList<EventDto>?> GetAllEventsAsync(CancellationToken cancellationToken);

    Task SetAllEventsAsync(IReadOnlyList<EventDto> events, CancellationToken cancellationToken);

    Task InvalidateAllEventsAsync(CancellationToken cancellationToken);
}
