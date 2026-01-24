using CommunityCar.Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunityCar.Infrastructure.Persistence.Configurations;

public class ErrorLogConfiguration : IEntityTypeConfiguration<ErrorLog>
{
    public void Configure(EntityTypeBuilder<ErrorLog> builder)
    {
        builder.ToTable("ErrorLogs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired();

        builder.Property(e => e.ErrorId)
            .IsRequired()
            .HasMaxLength(36);

        builder.HasIndex(e => e.ErrorId)
            .IsUnique();

        builder.Property(e => e.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.StackTrace)
            .HasColumnType("text");

        builder.Property(e => e.InnerException)
            .HasColumnType("text");

        builder.Property(e => e.Source)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Severity)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Error");

        builder.Property(e => e.Category)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("General");

        builder.Property(e => e.UserId)
            .HasMaxLength(36);

        builder.Property(e => e.UserAgent)
            .HasMaxLength(1000);

        builder.Property(e => e.IpAddress)
            .HasMaxLength(45); // IPv6 max length

        builder.Property(e => e.RequestPath)
            .HasMaxLength(2000);

        builder.Property(e => e.RequestMethod)
            .HasMaxLength(10);

        builder.Property(e => e.RequestHeaders)
            .HasColumnType("text");

        builder.Property(e => e.RequestBody)
            .HasColumnType("text");

        builder.Property(e => e.AdditionalData)
            .HasColumnType("text");

        builder.Property(e => e.IsResolved)
            .HasDefaultValue(false);

        builder.Property(e => e.ResolvedBy)
            .HasMaxLength(256);

        builder.Property(e => e.Resolution)
            .HasMaxLength(2000);

        builder.Property(e => e.OccurrenceCount)
            .HasDefaultValue(1);

        builder.Property(e => e.LastOccurrence)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt);

        // Indexes for performance
        builder.HasIndex(e => e.Severity);
        builder.HasIndex(e => e.Category);
        builder.HasIndex(e => e.IsResolved);
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.LastOccurrence);
        builder.HasIndex(e => e.UserId);

        // Relationships
        builder.HasMany(e => e.Occurrences)
            .WithOne(o => o.ErrorLog)
            .HasForeignKey(o => o.ErrorLogId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ErrorOccurrenceConfiguration : IEntityTypeConfiguration<ErrorOccurrence>
{
    public void Configure(EntityTypeBuilder<ErrorOccurrence> builder)
    {
        builder.ToTable("ErrorOccurrences");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired();

        builder.Property(e => e.ErrorLogId)
            .IsRequired();

        builder.Property(e => e.UserId)
            .HasMaxLength(36);

        builder.Property(e => e.SessionId)
            .HasMaxLength(100);

        builder.Property(e => e.IpAddress)
            .HasMaxLength(45);

        builder.Property(e => e.UserAgent)
            .HasMaxLength(1000);

        builder.Property(e => e.RequestPath)
            .HasMaxLength(2000);

        builder.Property(e => e.AdditionalContext)
            .HasColumnType("text");

        builder.Property(e => e.OccurredAt)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt);

        // Indexes
        builder.HasIndex(e => e.ErrorLogId);
        builder.HasIndex(e => e.OccurredAt);
        builder.HasIndex(e => e.UserId);
    }
}