using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Features.Community.Guides.ViewModels;

#region View Models

public class GuideResultVM
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? GuideId { get; set; }
}

#endregion

#region Request Models

public class CreateGuideRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    
    // Arabic Localization
    public string? TitleAr { get; set; }
    public string? ContentAr { get; set; }
    public string? SummaryAr { get; set; }
    
    public string? Category { get; set; }
    public GuideDifficulty Difficulty { get; set; } = GuideDifficulty.Beginner;
    public int EstimatedMinutes { get; set; } = 30;
    public string? ThumbnailUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Prerequisites { get; set; } = new();
    public List<string> RequiredTools { get; set; } = new();
}

public class UpdateGuideRequest
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

public class GuideFilterVM
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

#endregion
