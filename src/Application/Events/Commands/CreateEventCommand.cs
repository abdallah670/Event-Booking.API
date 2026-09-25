using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Events.Commands;

public sealed record CreateEventCommand(
    int CategoryId,
    string Title,
    string Description,
    string Location,
    DateTimeOffset StartsAt,
    int TotalSeats,
    Guid OrganizerId) : IRequest<CreateEventResult>;

public sealed record CreateEventResult(Guid Id);

public sealed class CreateEventCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateEventCommand, CreateEventResult>
{
    public async Task<CreateEventResult> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        if (request.StartsAt <= DateTimeOffset.UtcNow)
        {
            throw new ConflictException("An event must start in the future.");
        }

        var categoryExists = await dbContext.Categories
            .AnyAsync(category => category.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
        {
            throw new NotFoundException("Category not found.");
        }

        var eventItem = new Event
        {
            Id = Guid.NewGuid(),
            OrganizerId = request.OrganizerId,
            CategoryId = request.CategoryId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Location = request.Location.Trim(),
            StartsAt = request.StartsAt,
            TotalSeats = request.TotalSeats,
            AvailableSeats = request.TotalSeats,
            Status = EventStatus.Draft
        };

        dbContext.Events.Add(eventItem);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateEventResult(eventItem.Id);
    }
}