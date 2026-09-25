using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");
        builder.HasKey(eventItem => eventItem.Id);
        builder.Property(eventItem => eventItem.Title).HasMaxLength(200).IsRequired();
        builder.Property(eventItem => eventItem.Description).HasMaxLength(4000).IsRequired();
        builder.Property(eventItem => eventItem.Location).HasMaxLength(300).IsRequired();
        builder.Property(eventItem => eventItem.TotalSeats).IsRequired();
        builder.Property(eventItem => eventItem.AvailableSeats).IsRequired();
        builder.Property(eventItem => eventItem.Status).IsRequired();
        builder.Property(eventItem => eventItem.RowVersion).IsRowVersion();
        builder.HasIndex(eventItem => new { eventItem.Status, eventItem.StartsAt });
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(eventItem => eventItem.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(eventItem => eventItem.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}