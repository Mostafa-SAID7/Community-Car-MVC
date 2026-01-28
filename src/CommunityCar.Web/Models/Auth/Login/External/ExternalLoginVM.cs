using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Models.Auth.Login.External;

public class ExternalLoginVM
{
    public string Provider { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
}