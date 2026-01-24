using System.ComponentModel.DataAnnotations;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Features.News.ViewModels;

public class NewsCreateVM
{
    [Required]
    [StringLength(200)]
    public string Headline { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Summary { get; set; }

    [Url]
    public string? ImageUrl { get; set; }

    public List<string> ImageUrls { get; set; } = new();

    [Required]
    public NewsCategory Category { get; set; }

    [StringLength(50)]
    public string? CarMake { get; set; }

    [StringLength(50)]
    public string? CarModel { get; set; }

    [Range(1900, 2030)]
    public int? CarYear { get; set; }

    public string? Tags { get; set; }

    [StringLength(100)]
    public string? Source { get; set; }

    [Url]
    public string? SourceUrl { get; set; }

    public bool IsFeatured { get; set; }
    public bool IsPinned { get; set; }
    public bool PublishImmediately { get; set; } = true;

    [StringLength(60)]
    public string? MetaTitle { get; set; }

    [StringLength(160)]
    public string? MetaDescription { get; set; }

    public List<string> GetTagsList()
    {
        if (string.IsNullOrWhiteSpace(Tags))
            return new List<string>();

        return Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(t => t.Trim())
                  .Where(t => !string.IsNullOrWhiteSpace(t))
                  .ToList();
    }
}