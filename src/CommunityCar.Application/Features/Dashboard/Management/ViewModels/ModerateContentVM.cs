using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Dashboard.Management.ViewModels;

public class ModerateContentVM
{
    [Required]
    public Guid ContentId { get; set; }
    
    [Required]
    public string Action { get; set; } = string.Empty; // Approve, Reject, Remove, Flag
    
    [Required]
    public string Reason { get; set; } = string.Empty;
    
    public string? Notes { get; set; }
    public bool NotifyAuthor { get; set; } = true;
    public bool NotifyReporters { get; set; } = false;
}