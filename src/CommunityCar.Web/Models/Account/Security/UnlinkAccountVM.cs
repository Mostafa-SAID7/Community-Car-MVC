using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Account.Security;

public class UnlinkAccountVM
{
    [Required]
    public string Provider { get; set; } = string.Empty;
}