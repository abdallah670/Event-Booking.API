using Application.Common.Interfaces;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Events.Queries;

public sealed record GetUpcomingEventsQuery(
    int? CategoryId,
    DateTimeOffset? From,
    int Page = 1,
    int PageSize = 10) : IRequest<PagedResult<EventSummary>>;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public sealed record EventSummary(
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

public sealed class GetUpcomingEventsQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetUpcomingEventsQuery, PagedResult<EventSummary>>
{
    public async Task<PagedResult<EventSummary>> Handle(
        GetUpcomingEventsQuery request,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var query = dbContext.Events
            .AsNoTracking()
            .Where(eventItem => eventItem.Status == EventStatus.Published && eventItem.StartsAt > now);

        if (request.CategoryId.HasValue)
        {
            query = query.Where(eventItem => eventItem.CategoryId == request.CategoryId.Value);
        }

        if (request.From.HasValue)
        {
            query = query.Where(eventItem => eventItem.StartsAt >= request.From.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await (from eventItem in query
                           join category in dbContext.Categories.AsNoTracking()
                               on eventItem.CategoryId equals category.Id
                           orderby eventItem.StartsAt, eventItem.Title
                           select new EventSummary(
                               eventItem.Id,
                               eventItem.Title,
                               eventItem.Description,
                               eventItem.Location,
                               eventItem.StartsAt,
                               eventItem.AvailableSeats,
                               eventItem.TotalSeats,
                               eventItem.CategoryId,
                               category.Name,
                               eventItem.Status))
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<EventSummary>(items, request.Page, request.PageSize, totalCount);
    }
}

public sealed class GetUpcomingEventsQueryValidator : AbstractValidator<GetUpcomingEventsQuery>
{
    public GetUpcomingEventsQueryValidator()
    {
        RuleFor(query => query.CategoryId)
            .GreaterThan(0)
            .When(query => query.CategoryId.HasValue);
        RuleFor(query => query.Page)
            .GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100);
    }
}