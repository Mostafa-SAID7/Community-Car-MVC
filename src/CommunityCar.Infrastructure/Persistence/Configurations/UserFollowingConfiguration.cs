using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CommunityCar.Domain.Entities.Account.Profile;

namespace CommunityCar.Infrastructure.Persistence.Configurations;

public class UserFollowingConfiguration : IEntityTypeConfiguration<UserFollowing>
{
    public void Configure(EntityTypeBuilder<UserFollowing> builder)
    {
        builder.ToTable("UserFollowings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FollowerId)
            .IsRequired();

        builder.Property(x => x.FollowedUserId)
            .IsRequired();

        builder.Property(x => x.FollowedAt)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.FollowReason)
            .HasMaxLength(500);

        builder.Property(x => x.NotificationsEnabled)
            .HasDefaultValue(true);

        builder.Property(x => x.LastInteractionAt)
            .IsRequired(false);

        builder.Property(x => x.InteractionCount)
            .HasDefaultValue(0);

        // Indexes
        builder.HasIndex(x => x.FollowerId)
            .HasDatabaseName("IX_UserFollowings_FollowerId");

        builder.HasIndex(x => x.FollowedUserId)
            .HasDatabaseName("IX_UserFollowings_FollowedUserId");

        builder.HasIndex(x => new { x.FollowerId, x.FollowedUserId })
            .IsUnique()
            .HasDatabaseName("IX_UserFollowings_FollowerId_FollowedUserId");

        builder.HasIndex(x => x.FollowedAt)
            .HasDatabaseName("IX_UserFollowings_FollowedAt");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_UserFollowings_IsActive");

        // Prevent self-following
        builder.HasCheckConstraint("CK_UserFollowings_NoSelfFollow", "[FollowerId] != [FollowedUserId]");
    }
}
