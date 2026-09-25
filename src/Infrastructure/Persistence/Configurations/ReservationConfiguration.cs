using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");
        builder.HasKey(reservation => reservation.Id);
        builder.Property(reservation => reservation.Quantity).IsRequired();
        builder.Property(reservation => reservation.Status).IsRequired();
        builder.HasIndex(reservation => new { reservation.Status, reservation.ExpiresAt });
        builder.HasOne<Event>()
            .WithMany()
            .HasForeignKey(reservation => reservation.EventId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(reservation => reservation.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}