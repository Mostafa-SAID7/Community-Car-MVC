using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Account.Management;

public class DeleteAccountVM
{
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    [Display(Name = "Reason (Optional)")]
    public string? Reason { get; set; }

    [Required(ErrorMessage = "You must confirm account deletion")]
    [Display(Name = "I understand that this action cannot be undone")]
    public bool ConfirmDeletion { get; set; }
}