using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Core;

public class UserActivityVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserAvatar { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty; // Post, Comment, Like, Share, etc.
    public string ActivityDescription { get; set; } = string.Empty;
    public Guid? ContentId { get; set; }
    public string? ContentTitle { get; set; }
    public string? ContentType { get; set; }
    public string? ContentUrl { get; set; }
    public DateTime ActivityDate { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}
