using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CommunityCar.Domain.Entities.Account;

namespace CommunityCar.Infrastructure.Persistence.Configurations;

public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
{
    public void Configure(EntityTypeBuilder<UserActivity> builder)
    {
        builder.ToTable("UserActivities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.ActivityType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EntityId)
            .IsRequired(false);

        builder.Property(x => x.EntityTitle)
            .HasMaxLength(500);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.Metadata)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.IpAddress)
            .HasMaxLength(45); // IPv6 support

        builder.Property(x => x.UserAgent)
            .HasMaxLength(1000);

        builder.Property(x => x.Location)
            .HasMaxLength(200);

        builder.Property(x => x.ActivityDate)
            .IsRequired();

        builder.Property(x => x.Duration)
            .HasDefaultValue(0);

        builder.Property(x => x.IsTracked)
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_UserActivities_UserId");

        builder.HasIndex(x => x.ActivityDate)
            .HasDatabaseName("IX_UserActivities_ActivityDate");

        builder.HasIndex(x => new { x.UserId, x.ActivityType })
            .HasDatabaseName("IX_UserActivities_UserId_ActivityType");

        builder.HasIndex(x => new { x.EntityType, x.EntityId })
            .HasDatabaseName("IX_UserActivities_EntityType_EntityId");
    }
}
