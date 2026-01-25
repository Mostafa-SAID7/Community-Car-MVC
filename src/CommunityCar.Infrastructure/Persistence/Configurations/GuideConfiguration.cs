using CommunityCar.Domain.Entities.Community.Guides;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace CommunityCar.Infrastructure.Persistence.Configurations;

public class GuideConfiguration : IEntityTypeConfiguration<Guide>
{
    public void Configure(EntityTypeBuilder<Guide> builder)
    {
        builder.ToTable("Guides");

        builder.Property(g => g.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
            .HasColumnName("TagsJson")
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(g => g.Prerequisites)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
            .HasColumnName("PrerequisitesJson")
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(g => g.RequiredTools)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
            .HasColumnName("RequiredToolsJson")
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        // Arabic Localization fields (optional configuration for max length)
        builder.Property(g => g.TitleAr)
            .HasMaxLength(500);
        
        builder.Property(g => g.SummaryAr)
            .HasMaxLength(1000);
    }
}
