using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Account.Security;

public class LinkAccountVM
{
    [Required]
    public string Token { get; set; } = string.Empty;
}