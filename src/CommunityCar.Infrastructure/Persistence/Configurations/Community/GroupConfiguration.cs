using CommunityCar.Domain.Entities.Community.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace CommunityCar.Infrastructure.Persistence.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("Groups");

        builder.Property(g => g.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
            .HasColumnName("TagsJson")
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

