using System.Security.Claims;
using Application.Events.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/events")]
public sealed class EventsController(IMediator mediator) : ControllerBase
{
    /// <summary>Creates a draft event for the authenticated organizer.</summary>
    [HttpPost]
    [Authorize(Policy = "OrganizerOnly")]
    [ProducesResponseType(typeof(CreateEventResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(CreateEventRequest request, CancellationToken cancellationToken)
    {
        var organizerId = GetCurrentUserId();
        var result = await mediator.Send(
            new CreateEventCommand(
                request.CategoryId,
                request.Title,
                request.Description,
                request.Location,
                request.StartsAt,
                request.TotalSeats,
                organizerId),
            cancellationToken);

        return Created($"/api/events/{result.Id}", result);
    }

    /// <summary>Publishes an organizer's draft event.</summary>
    [HttpPut("{id:guid}/publish")]
    [Authorize(Policy = "OrganizerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new PublishEventCommand(id, GetCurrentUserId()),
            cancellationToken);

        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(value ?? throw new InvalidOperationException("User identifier claim is missing."));
    }
}

public sealed record CreateEventRequest(
    int CategoryId,
    string Title,
    string Description,
    string Location,
    DateTimeOffset StartsAt,
    int TotalSeats);