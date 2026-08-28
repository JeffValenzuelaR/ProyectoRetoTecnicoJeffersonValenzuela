using EventService.Application.Common.Dtos;
using EventService.Application.Events.Commands.CreateEvent;
using EventService.Application.Events.Queries.GetEventById;
using EventService.Application.Events.Queries.GetEvents;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventService.Api.Controllers;

[ApiController]
[Route("events")]
public sealed class EventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<EventDto>> Create(
        [FromBody] CreateEventCommand command,
        CancellationToken cancellationToken)
    {
        var created = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyList<EventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> GetAll(CancellationToken cancellationToken)
    {
        var events = await _mediator.Send(new GetEventsQuery(), cancellationToken);
        return Ok(events);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEventByIdQuery(id), cancellationToken);
        return Ok(result);
    }
}
