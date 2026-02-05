using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Media.SoftDelete.ViewModels;

public class PermanentDeleteVM
{
    [Required]
    public List<Guid> ContentIds { get; set; } = new();
    
    [Required]
    public string Reason { get; set; } = string.Empty;
    
    public bool ConfirmPermanentDeletion { get; set; } = false;
}





