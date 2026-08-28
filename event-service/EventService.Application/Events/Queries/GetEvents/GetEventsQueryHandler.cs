using AutoMapper;
using EventService.Application.Common.Dtos;
using EventService.Application.Common.Interfaces;
using MediatR;

namespace EventService.Application.Events.Queries.GetEvents;

public sealed class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, IReadOnlyList<EventDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventCacheService _eventCacheService;
    private readonly IMapper _mapper;

    public GetEventsQueryHandler(
        IEventRepository eventRepository,
        IEventCacheService eventCacheService,
        IMapper mapper)
    {
        _eventRepository = eventRepository;
        _eventCacheService = eventCacheService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<EventDto>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        var cached = await _eventCacheService.GetAllEventsAsync(cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var events = await _eventRepository.GetAllAsync(cancellationToken);
        var dtos = _mapper.Map<IReadOnlyList<EventDto>>(events);

        await _eventCacheService.SetAllEventsAsync(dtos, cancellationToken);

        return dtos;
    }
}
