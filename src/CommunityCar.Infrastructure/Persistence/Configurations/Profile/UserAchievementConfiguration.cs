using CommunityCar.Domain.Entities.Profile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunityCar.Infrastructure.Persistence.Configurations;

public class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
{
    public void Configure(EntityTypeBuilder<UserAchievement> builder)
    {
        builder.ToTable("UserAchievements");

        builder.Property(a => a.Title).HasMaxLength(200);
        builder.Property(a => a.TitleAr).HasMaxLength(200);
        builder.Property(a => a.Description).HasMaxLength(500);
        builder.Property(a => a.DescriptionAr).HasMaxLength(500);
    }
}
