using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Community.SoftDelete.ViewModels;

public class RestoreContentVM
{
    [Required]
    public Guid ContentId { get; set; }
    
    [Required]
    public string ContentType { get; set; } = string.Empty;
    
    public string? RestoreReason { get; set; }
    public bool NotifyAuthor { get; set; } = true;
    public bool RestoreRelatedContent { get; set; } = false;
}