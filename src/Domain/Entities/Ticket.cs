namespace Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset IssuedAt { get; set; }
}