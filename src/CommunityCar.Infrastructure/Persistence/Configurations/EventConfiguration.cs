using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CommunityCar.Domain.Entities.Community.Events;

namespace CommunityCar.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.Location)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.LocationDetails)
            .HasMaxLength(1000);

        builder.Property(e => e.OrganizerId)
            .IsRequired();

        builder.Property(e => e.AttendeeCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.MaxAttendees)
            .IsRequired(false);

        builder.Property(e => e.RequiresApproval)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.TicketPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);

        builder.Property(e => e.TicketInfo)
            .HasMaxLength(1000);

        builder.Property(e => e.IsPublic)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.IsCancelled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.ExternalUrl)
            .HasMaxLength(500);

        builder.Property(e => e.ContactInfo)
            .HasMaxLength(500);

        builder.Property(e => e.Latitude)
            .HasColumnType("decimal(10,8)")
            .IsRequired(false);

        builder.Property(e => e.Longitude)
            .HasColumnType("decimal(11,8)")
            .IsRequired(false);

        // Configure collections as JSON
        builder.Property(e => e.Tags)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasColumnName("Tags");

        builder.Property(e => e.ImageUrls)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasColumnName("ImageUrls");

        // Indexes
        builder.HasIndex(e => e.StartTime);
        builder.HasIndex(e => e.EndTime);
        builder.HasIndex(e => e.OrganizerId);
        builder.HasIndex(e => e.IsPublic);
        builder.HasIndex(e => e.IsCancelled);
        builder.HasIndex(e => new { e.StartTime, e.EndTime });
    }
}