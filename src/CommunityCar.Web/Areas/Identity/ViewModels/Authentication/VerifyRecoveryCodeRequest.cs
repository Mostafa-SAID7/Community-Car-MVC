namespace CommunityCar.Web.Areas.Identity.ViewModels.Authentication;

public class VerifyRecoveryCodeRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
}
