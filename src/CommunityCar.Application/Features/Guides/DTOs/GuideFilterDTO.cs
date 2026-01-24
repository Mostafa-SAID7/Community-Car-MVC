using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Features.Guides.DTOs;

public class GuideFilterDTO
{
    public string? Search { get; set; }
    public string? Category { get; set; }
    public GuideDifficulty? Difficulty { get; set; }
    public string? Tag { get; set; }
    public bool? IsVerified { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsPublished { get; set; }
    public Guid? AuthorId { get; set; }
    public string SortBy { get; set; } = "newest";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}