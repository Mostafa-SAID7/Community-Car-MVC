using CommunityCar.Domain.Enums;

namespace CommunityCar.Application.Features.Guides.ViewModels;

public class GuideCreateVM
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Category { get; set; }
    public GuideDifficulty Difficulty { get; set; } = GuideDifficulty.Beginner;
    public int EstimatedMinutes { get; set; } = 30;
    public string? ThumbnailUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string TagsInput { get; set; } = string.Empty;
    public string PrerequisitesInput { get; set; } = string.Empty;
    public string RequiredToolsInput { get; set; } = string.Empty;
    public bool PublishImmediately { get; set; }
    
    public List<string> AvailableCategories { get; set; } = new()
    {
        "Maintenance", "Engine", "Brakes", "Electrical", "Bodywork", 
        "Interior", "Transmission", "Suspension", "Performance", "Restoration"
    };
}


