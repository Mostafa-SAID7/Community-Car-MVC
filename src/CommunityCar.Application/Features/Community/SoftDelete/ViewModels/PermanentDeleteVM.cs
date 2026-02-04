using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Community.SoftDelete.ViewModels;

public class PermanentDeleteVM
{
    [Required]
    public List<Guid> ContentIds { get; set; } = new();
    
    [Required]
    public string Reason { get; set; } = string.Empty;
    
    public bool ConfirmPermanentDeletion { get; set; } = false;
}