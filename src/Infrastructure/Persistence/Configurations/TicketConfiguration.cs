using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");
        builder.HasKey(ticket => ticket.Id);
        builder.Property(ticket => ticket.Code).HasMaxLength(100).IsRequired();
        builder.HasIndex(ticket => ticket.Code).IsUnique();
        builder.HasIndex(ticket => ticket.ReservationId).IsUnique();
        builder.HasOne<Reservation>()
            .WithOne()
            .HasForeignKey<Ticket>(ticket => ticket.ReservationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}