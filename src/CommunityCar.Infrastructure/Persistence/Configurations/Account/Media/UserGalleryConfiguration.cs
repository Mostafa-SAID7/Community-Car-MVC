using CommunityCar.Domain.Entities.Account.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunityCar.Infrastructure.Persistence.Configurations.Account.Media;

public class UserGalleryConfiguration : IEntityTypeConfiguration<UserGallery>
{
    public void Configure(EntityTypeBuilder<UserGallery> builder)
    {
        builder.ToTable("UserGalleries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.MediaUrl)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.ThumbnailUrl)
            .HasMaxLength(2000);

        builder.Property(x => x.MediaType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Tags)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.ViewCount)
            .HasDefaultValue(0);

        builder.Property(x => x.LikeCount)
            .HasDefaultValue(0);

        builder.Property(x => x.IsPublic)
            .HasDefaultValue(true);

        builder.Property(x => x.IsFeatured)
            .HasDefaultValue(false);

        builder.Property(x => x.UploadedAt)
            .IsRequired();

        // Indexes for performance
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_UserGalleries_UserId");

        builder.HasIndex(x => x.UploadedAt)
            .HasDatabaseName("IX_UserGalleries_UploadedAt");

        builder.HasIndex(x => new { x.UserId, x.IsPublic })
            .HasDatabaseName("IX_UserGalleries_UserId_IsPublic");

        builder.HasIndex(x => new { x.UserId, x.IsFeatured })
            .HasDatabaseName("IX_UserGalleries_UserId_IsFeatured");

        builder.HasIndex(x => x.MediaType)
            .HasDatabaseName("IX_UserGalleries_MediaType");

        builder.HasIndex(x => x.ViewCount)
            .HasDatabaseName("IX_UserGalleries_ViewCount");

        builder.HasIndex(x => x.LikeCount)
            .HasDatabaseName("IX_UserGalleries_LikeCount");

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