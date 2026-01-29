using CommunityCar.Domain.Entities.Account.Profile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunityCar.Infrastructure.Persistence.Configurations.Account.Activity;

public class UserProfileViewConfiguration : IEntityTypeConfiguration<UserProfileView>
{
    public void Configure(EntityTypeBuilder<UserProfileView> builder)
    {
        builder.ToTable("UserProfileViews");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ViewerId)
            .IsRequired();

        builder.Property(x => x.ProfileUserId)
            .IsRequired();

        builder.Property(x => x.ViewedAt)
            .IsRequired();

        builder.Property(x => x.ViewerIpAddress)
            .HasMaxLength(45); // IPv6 max length

        builder.Property(x => x.ViewerUserAgent)
            .HasMaxLength(500);

        builder.Property(x => x.ViewerLocation)
            .HasMaxLength(200);

        builder.Property(x => x.ReferrerUrl)
            .HasMaxLength(2000);

        builder.Property(x => x.ViewSource)
            .HasMaxLength(50);

        builder.Property(x => x.ViewDuration)
            .IsRequired();

        builder.Property(x => x.IsAnonymous)
            .IsRequired();

        // Indexes for performance
        builder.HasIndex(x => x.ProfileUserId)
            .HasDatabaseName("IX_UserProfileViews_ProfileUserId");

        builder.HasIndex(x => x.ViewerId)
            .HasDatabaseName("IX_UserProfileViews_ViewerId");

        builder.HasIndex(x => x.ViewedAt)
            .HasDatabaseName("IX_UserProfileViews_ViewedAt");

        builder.HasIndex(x => new { x.ViewerId, x.ProfileUserId })
            .HasDatabaseName("IX_UserProfileViews_ViewerId_ProfileUserId");

        builder.HasIndex(x => new { x.ProfileUserId, x.ViewedAt })
            .HasDatabaseName("IX_UserProfileViews_ProfileUserId_ViewedAt");

        builder.HasIndex(x => x.ViewSource)
            .HasDatabaseName("IX_UserProfileViews_ViewSource");

        // Foreign key relationships (optional - depends on your domain design)
        // Note: We don't enforce FK constraints here because ViewerId can be Guid.Empty for anonymous views
        
        // Soft delete support
        builder.HasQueryFilter(x => !x.IsDeleted);

        // Audit fields
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(256);

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(256);

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.DeletedAt);

        builder.Property(x => x.DeletedBy)
            .HasMaxLength(256);
    }
}