using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Authentication;

public class CreateTokenRequest
{
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}
