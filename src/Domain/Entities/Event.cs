using Domain.Enums;

namespace Domain.Entities;

public class Event
{
    public Guid Id { get; set; }
    public Guid OrganizerId { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTimeOffset StartsAt { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Draft;
    public byte[] RowVersion { get; set; } = [];
}