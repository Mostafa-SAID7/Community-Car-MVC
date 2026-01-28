using CommunityCar.Domain.Enums.Community;
using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.News.DTOs;

public class CreateNewsRequest
{
    [Required]
    [StringLength(200)]
    public string Headline { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Summary { get; set; }

    [StringLength(200)]
    public string? HeadlineAr { get; set; }

    public string? BodyAr { get; set; }

    [StringLength(500)]
    public string? SummaryAr { get; set; }

    public string? ImageUrl { get; set; }

    [Required]
    public Guid AuthorId { get; set; }

    public NewsCategory Category { get; set; } = NewsCategory.General;

    public string? Source { get; set; }

    public string? SourceUrl { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public List<string> Tags { get; set; } = new();

    public List<string> ImageUrls { get; set; } = new();

    public string? CarMake { get; set; }

    public string? CarModel { get; set; }

    public int? CarYear { get; set; }

    public bool PublishImmediately { get; set; } = false;

    public bool IsFeatured { get; set; } = false;

    public bool IsPinned { get; set; } = false;
}


