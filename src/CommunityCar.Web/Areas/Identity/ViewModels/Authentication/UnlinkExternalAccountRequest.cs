using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Authentication;

public class UnlinkExternalAccountRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
}
