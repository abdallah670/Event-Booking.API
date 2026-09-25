using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Events.Commands;

public sealed record PublishEventCommand(Guid EventId, Guid OrganizerId) : IRequest<Unit>;

public sealed class PublishEventCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<PublishEventCommand, Unit>
{
    public async Task<Unit> Handle(PublishEventCommand request, CancellationToken cancellationToken)
    {
        var eventItem = await dbContext.Events
            .SingleOrDefaultAsync(candidate => candidate.Id == request.EventId, cancellationToken);

        if (eventItem is null)
        {
            throw new NotFoundException("Event not found.");
        }

        if (eventItem.OrganizerId != request.OrganizerId)
        {
            throw new ForbiddenException("Only the event owner can publish this event.");
        }

        if (eventItem.Status != EventStatus.Draft)
        {
            throw new ConflictException("Only draft events can be published.");
        }

        if (eventItem.StartsAt <= DateTimeOffset.UtcNow)
        {
            throw new ConflictException("An event that already started cannot be published.");
        }

        eventItem.Status = EventStatus.Published;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}