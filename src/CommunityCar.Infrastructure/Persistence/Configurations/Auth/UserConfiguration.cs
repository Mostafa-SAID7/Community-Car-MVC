using CommunityCar.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunityCar.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.FullName).HasMaxLength(100);
        builder.Property(u => u.FirstName).HasMaxLength(50);
        builder.Property(u => u.LastName).HasMaxLength(50);
        builder.Property(u => u.Bio).HasMaxLength(500);
        builder.Property(u => u.BioAr).HasMaxLength(500);
        builder.Property(u => u.City).HasMaxLength(100);
        builder.Property(u => u.CityAr).HasMaxLength(100);
        builder.Property(u => u.Country).HasMaxLength(100);
        builder.Property(u => u.CountryAr).HasMaxLength(100);
    }
}

