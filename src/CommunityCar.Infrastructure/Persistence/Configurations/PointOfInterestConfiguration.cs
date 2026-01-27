using CommunityCar.Domain.Entities.Community.Maps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace CommunityCar.Infrastructure.Persistence.Configurations;

public class PointOfInterestConfiguration : IEntityTypeConfiguration<PointOfInterest>
{
    public void Configure(EntityTypeBuilder<PointOfInterest> builder)
    {
        builder.ToTable("PointsOfInterest");

        builder.Property(p => p.ImageUrls)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
            .HasColumnName("ImageUrlsJson")
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

