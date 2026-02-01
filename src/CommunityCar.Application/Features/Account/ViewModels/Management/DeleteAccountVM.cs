using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class DeleteAccountVM
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;

    [Display(Name = "I confirm that I want to permanently delete my account and all associated data.")]
    public bool ConfirmDeletion { get; set; }
}