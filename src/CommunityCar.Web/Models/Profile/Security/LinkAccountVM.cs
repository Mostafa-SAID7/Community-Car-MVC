using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Profile.Security;

public class LinkAccountVM
{
    [Required]
    public string Token { get; set; } = string.Empty;
}