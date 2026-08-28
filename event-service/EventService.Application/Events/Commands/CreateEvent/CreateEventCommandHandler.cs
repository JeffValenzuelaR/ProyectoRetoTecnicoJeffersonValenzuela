using AutoMapper;
using EventService.Application.Common.Dtos;
using EventService.Application.Common.Interfaces;
using EventService.Domain.Entities;
using MediatR;

namespace EventService.Application.Events.Commands.CreateEvent;

public sealed class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, EventDto>
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventCreatedPublisher _eventCreatedPublisher;
    private readonly IEventCacheService _eventCacheService;
    private readonly IMapper _mapper;

    public CreateEventCommandHandler(
        IEventRepository eventRepository,
        IEventCreatedPublisher eventCreatedPublisher,
        IEventCacheService eventCacheService,
        IMapper mapper)
    {
        _eventRepository = eventRepository;
        _eventCreatedPublisher = eventCreatedPublisher;
        _eventCacheService = eventCacheService;
        _mapper = mapper;
    }

    public async Task<EventDto> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var @event = Event.Create(request.Name, request.Date, request.Venue);

        foreach (var zoneDto in request.Zones)
        {
            var zone = Zone.Create(zoneDto.Name, zoneDto.Price, zoneDto.Capacity);
            @event.AddZone(zone);
        }

        await using var transaction = await _eventRepository.BeginTransactionAsync(cancellationToken);
        try
        {
            await _eventRepository.AddAsync(@event, cancellationToken);

            await _eventCreatedPublisher.PublishEventCreatedAsync(@event.Id, @event.Name, cancellationToken);

            await _eventRepository.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        await _eventCacheService.InvalidateAllEventsAsync(cancellationToken);

        return _mapper.Map<EventDto>(@event);
    }
}
