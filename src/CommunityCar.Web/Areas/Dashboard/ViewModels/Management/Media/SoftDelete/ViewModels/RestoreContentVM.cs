using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Media.SoftDelete.ViewModels;

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





