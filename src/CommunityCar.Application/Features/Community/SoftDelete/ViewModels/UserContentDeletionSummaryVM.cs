namespace CommunityCar.Application.Features.Community.SoftDelete.ViewModels;

/// <summary>
/// User content deletion summary
/// </summary>
public class UserContentDeletionSummaryVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public int TotalPosts { get; set; }
    public int DeletedPosts { get; set; }
    public int TotalStories { get; set; }
    public int DeletedStories { get; set; }
    public int TotalGroups { get; set; }
    public int DeletedGroups { get; set; }
    public int TotalComments { get; set; }
    public int DeletedComments { get; set; }
    public DateTime? LastDeletionDate { get; set; }
    public string? LastDeletionBy { get; set; }
    public bool HasActiveContent => (TotalPosts - DeletedPosts) > 0 || 
                                   (TotalStories - DeletedStories) > 0 || 
                                   (TotalGroups - DeletedGroups) > 0 || 
                                   (TotalComments - DeletedComments) > 0;
}