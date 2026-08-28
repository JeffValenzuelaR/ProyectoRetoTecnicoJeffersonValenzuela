using EventService.Application.Common.Dtos;
using MediatR;

namespace EventService.Application.Events.Queries.GetEvents;

public sealed class GetEventsQuery : IRequest<IReadOnlyList<EventDto>>
{
}
