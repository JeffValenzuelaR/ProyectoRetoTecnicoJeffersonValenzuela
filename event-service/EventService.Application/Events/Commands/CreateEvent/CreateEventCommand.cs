using EventService.Application.Common.Dtos;
using MediatR;

namespace EventService.Application.Events.Commands.CreateEvent;

public sealed class CreateEventCommand : IRequest<EventDto>
{
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset Date { get; set; }
    public string Venue { get; set; } = string.Empty;
    public List<CreateZoneDto> Zones { get; set; } = new();
}
