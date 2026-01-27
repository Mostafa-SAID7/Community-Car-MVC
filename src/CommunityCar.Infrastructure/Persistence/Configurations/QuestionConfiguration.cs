using CommunityCar.Domain.Entities.Community.QA;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace CommunityCar.Infrastructure.Persistence.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");

        builder.Property(q => q.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(q => q.Body)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(q => q.TitleAr)
            .HasMaxLength(500);

        builder.Property(q => q.BodyAr)
            .HasMaxLength(5000);

        builder.Property(q => q.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
            .HasColumnName("TagsJson")
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

