using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CommunityCar.Domain.Entities.AI;

namespace CommunityCar.Infrastructure.Persistence.Configurations.AI;

public class AIModelConfiguration : IEntityTypeConfiguration<AIModel>
{
    public void Configure(EntityTypeBuilder<AIModel> builder)
    {
        builder.ToTable("AIModels");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Version)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.Accuracy)
            .HasPrecision(5, 2);

        builder.Property(x => x.Configuration)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.ModelPath)
            .HasMaxLength(500);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasIndex(x => x.Type);
        builder.HasIndex(x => x.Status);

        // Relationships
        builder.HasMany(x => x.TrainingJobs)
            .WithOne(x => x.AIModel)
            .HasForeignKey(x => x.AIModelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.TrainingHistories)
            .WithOne(x => x.AIModel)
            .HasForeignKey(x => x.AIModelId)
            .OnDelete(DeleteBehavior.Cascade);

        // Soft delete filter
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class TrainingJobConfiguration : IEntityTypeConfiguration<TrainingJob>
{
    public void Configure(EntityTypeBuilder<TrainingJob> builder)
    {
        builder.ToTable("TrainingJobs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.JobName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.Parameters)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(x => x.ResultAccuracy)
            .HasPrecision(5, 2);

        builder.Property(x => x.ResultMetrics)
            .HasColumnType("nvarchar(max)");

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.StartedAt);
        builder.HasIndex(x => x.Priority);

        // Soft delete filter
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class TrainingHistoryConfiguration : IEntityTypeConfiguration<TrainingHistory>
{
    public void Configure(EntityTypeBuilder<TrainingHistory> builder)
    {
        builder.ToTable("TrainingHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Version)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.InitialAccuracy)
            .HasPrecision(5, 2);

        builder.Property(x => x.FinalAccuracy)
            .HasPrecision(5, 2);

        builder.Property(x => x.Improvement)
            .HasPrecision(5, 2);

        builder.Property(x => x.Result)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.TrainingLog)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.Metrics)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.LearningRate)
            .HasPrecision(10, 8);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.TrainingDate);
        builder.HasIndex(x => x.Result);

        // Soft delete filter
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}