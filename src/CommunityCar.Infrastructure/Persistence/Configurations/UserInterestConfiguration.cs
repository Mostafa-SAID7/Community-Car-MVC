using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CommunityCar.Domain.Entities.Account;

namespace CommunityCar.Infrastructure.Persistence.Configurations;

public class UserInterestConfiguration : IEntityTypeConfiguration<UserInterest>
{
    public void Configure(EntityTypeBuilder<UserInterest> builder)
    {
        builder.ToTable("UserInterests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.SubCategory)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.InterestType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.InterestValue)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Score)
            .HasPrecision(18, 2)
            .HasDefaultValue(0.0);

        builder.Property(x => x.InteractionCount)
            .HasDefaultValue(0);

        builder.Property(x => x.LastInteraction)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.Source)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_UserInterests_UserId");

        builder.HasIndex(x => new { x.UserId, x.Category })
            .HasDatabaseName("IX_UserInterests_UserId_Category");

        builder.HasIndex(x => new { x.UserId, x.InterestType, x.InterestValue })
            .IsUnique()
            .HasDatabaseName("IX_UserInterests_UserId_InterestType_InterestValue");

        builder.HasIndex(x => x.Score)
            .HasDatabaseName("IX_UserInterests_Score");

        builder.HasIndex(x => x.LastInteraction)
            .HasDatabaseName("IX_UserInterests_LastInteraction");
    }
}
