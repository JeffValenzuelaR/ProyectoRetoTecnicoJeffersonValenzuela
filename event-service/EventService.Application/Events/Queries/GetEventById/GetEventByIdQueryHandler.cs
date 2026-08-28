using AutoMapper;
using EventService.Application.Common.Dtos;
using EventService.Application.Common.Exceptions;
using EventService.Application.Common.Interfaces;
using MediatR;

namespace EventService.Application.Events.Queries.GetEventById;

public sealed class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDto>
{
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;

    public GetEventByIdQueryHandler(IEventRepository eventRepository, IMapper mapper)
    {
        _eventRepository = eventRepository;
        _mapper = mapper;
    }

    public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var @event = await _eventRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw NotFoundException.ForEvent(request.Id);

        return _mapper.Map<EventDto>(@event);
    }
}
