using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Core;

public class UserInteractionVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserAvatar { get; set; } = string.Empty;
    public Guid TargetUserId { get; set; }
    public string TargetUserName { get; set; } = string.Empty;
    public string InteractionType { get; set; } = string.Empty; // Like, Comment, Share, Follow, etc.
    public Guid? ContentId { get; set; }
    public string? ContentTitle { get; set; }
    public string? ContentType { get; set; }
    public DateTime InteractionDate { get; set; }
    public string? Message { get; set; }
    public bool IsRead { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}
