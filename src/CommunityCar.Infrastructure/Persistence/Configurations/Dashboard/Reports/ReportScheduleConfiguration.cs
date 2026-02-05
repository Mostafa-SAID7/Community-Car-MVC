using CommunityCar.Domain.Entities.Dashboard.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunityCar.Infrastructure.Persistence.Configurations.Dashboard.Reports;

public class ReportScheduleConfiguration : IEntityTypeConfiguration<ReportSchedule>
{
    public void Configure(EntityTypeBuilder<ReportSchedule> builder)
    {
        builder.ToTable("ReportSchedules");

        builder.HasKey(rs => rs.Id);

        builder.Property(rs => rs.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(rs => rs.Description)
            .HasMaxLength(1000);

        builder.Property(rs => rs.ReportType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(rs => rs.Frequency)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(rs => rs.Format)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(rs => rs.CronExpression)
            .HasMaxLength(100);

        builder.Property(rs => rs.Recipients)
            .HasColumnType("nvarchar(max)");

        builder.Property(rs => rs.Template)
            .HasColumnType("nvarchar(max)");

        builder.Property(rs => rs.CreatedAt)
            .IsRequired();

        builder.Property(rs => rs.UpdatedAt)
            .IsRequired();

        builder.HasIndex(rs => rs.ReportType);
        builder.HasIndex(rs => rs.IsActive);
        builder.HasIndex(rs => rs.NextRun);
        builder.HasIndex(rs => rs.Frequency);

        // Relationships
        builder.HasOne(rs => rs.Report)
            .WithMany(r => r.Schedules)
            .HasForeignKey(rs => rs.ReportId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}