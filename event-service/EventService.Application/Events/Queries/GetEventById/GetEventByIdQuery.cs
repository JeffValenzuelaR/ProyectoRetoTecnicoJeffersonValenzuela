using EventService.Application.Common.Dtos;
using MediatR;

namespace EventService.Application.Events.Queries.GetEventById;

public sealed class GetEventByIdQuery : IRequest<EventDto>
{
    public Guid Id { get; }

    public GetEventByIdQuery(Guid id)
    {
        Id = id;
    }
}
