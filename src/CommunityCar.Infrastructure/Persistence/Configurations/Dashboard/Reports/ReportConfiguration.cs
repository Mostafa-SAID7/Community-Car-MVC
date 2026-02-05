using CommunityCar.Domain.Entities.Dashboard.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunityCar.Infrastructure.Persistence.Configurations.Dashboard.Reports;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("Reports");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(r => r.Description)
            .HasMaxLength(2000);

        builder.Property(r => r.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(r => r.Format)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(r => r.GeneratedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.FilePath)
            .HasMaxLength(1000);

        builder.Property(r => r.FileUrl)
            .HasMaxLength(1000);

        builder.Property(r => r.Parameters)
            .HasColumnType("nvarchar(max)");

        builder.Property(r => r.Data)
            .HasColumnType("nvarchar(max)");

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .IsRequired();

        builder.HasIndex(r => r.Type);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.CreatedAt);
        builder.HasIndex(r => r.ExpiresAt);

        // Relationships
        builder.HasMany(r => r.Schedules)
            .WithOne(rs => rs.Report)
            .HasForeignKey(rs => rs.ReportId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}