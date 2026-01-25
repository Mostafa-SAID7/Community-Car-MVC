using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Features.Guides.DTOs;

public class UpdateGuideDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    
    // Arabic Localization
    public string? TitleAr { get; set; }
    public string? ContentAr { get; set; }
    public string? SummaryAr { get; set; }
    
    public string? Category { get; set; }
    public GuideDifficulty Difficulty { get; set; }
    public int EstimatedMinutes { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Prerequisites { get; set; } = new();
    public List<string> RequiredTools { get; set; } = new();
}