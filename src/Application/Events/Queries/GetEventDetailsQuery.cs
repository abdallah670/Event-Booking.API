using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Events.Queries;

public sealed record GetEventDetailsQuery(Guid EventId) : IRequest<EventDetails>;

public sealed record EventDetails(
    Guid Id,
    string Title,
    string Description,
    string Location,
    DateTimeOffset StartsAt,
    int AvailableSeats,
    int TotalSeats,
    int CategoryId,
    string CategoryName,
    EventStatus Status);

public sealed class GetEventDetailsQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetEventDetailsQuery, EventDetails>
{
    public async Task<EventDetails> Handle(GetEventDetailsQuery request, CancellationToken cancellationToken)
    {
        var result = await (from eventItem in dbContext.Events.AsNoTracking()
                            join category in dbContext.Categories.AsNoTracking()
                                on eventItem.CategoryId equals category.Id
                            where eventItem.Id == request.EventId
                            select new
                            {
                                Event = eventItem,
                                CategoryName = category.Name
                            })
            .SingleOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            throw new NotFoundException("Event not found.");
        }

        return new EventDetails(
            result.Event.Id,
            result.Event.Title,
            result.Event.Description,
            result.Event.Location,
            result.Event.StartsAt,
            result.Event.AvailableSeats,
            result.Event.TotalSeats,
            result.Event.CategoryId,
            result.CategoryName,
            result.Event.Status);
    }
}

public sealed class GetEventDetailsQueryValidator : AbstractValidator<GetEventDetailsQuery>
{
    public GetEventDetailsQueryValidator()
    {
        RuleFor(query => query.EventId).NotEmpty();
    }
}