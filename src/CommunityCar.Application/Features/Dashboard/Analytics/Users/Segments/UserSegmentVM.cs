namespace CommunityCar.Application.Features.Dashboard.Analytics.Users.Segments;

/// <summary>
/// User segment view model
/// </summary>
public class UserSegmentVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Criteria { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Dictionary<string, object> SegmentRules { get; set; } = new();
}