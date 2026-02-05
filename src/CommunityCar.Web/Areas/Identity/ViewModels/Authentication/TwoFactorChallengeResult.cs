namespace CommunityCar.Web.Areas.Identity.ViewModels.Authentication;

public class TwoFactorChallengeResult
{
    public bool RequiresTwoFactor { get; set; }
    public string[] AvailableProviders { get; set; } = Array.Empty<string>();
    public string ChallengeToken { get; set; } = string.Empty;
}
