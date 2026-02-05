namespace CommunityCar.Web.Areas.Identity.ViewModels.Authentication;

public class TwoFactorSetupRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
}
