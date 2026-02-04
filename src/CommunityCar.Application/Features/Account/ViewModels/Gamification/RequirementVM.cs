namespace CommunityCar.Application.Features.Account.ViewModels.Gamification;

/// <summary>
/// ViewModel for individual requirements
/// </summary>
public class RequirementVM
{
    public string Type { get; set; } = string.Empty; // Posts, Comments, Likes, etc.
    public int Required { get; set; }
    public int Current { get; set; }
    public bool IsCompleted { get; set; }
    public string Description { get; set; } = string.Empty;
}