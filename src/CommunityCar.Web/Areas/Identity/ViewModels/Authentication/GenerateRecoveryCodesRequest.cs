namespace CommunityCar.Web.Areas.Identity.ViewModels.Authentication;

public class GenerateRecoveryCodesRequest
{
    public Guid UserId { get; set; }
    public string Password { get; set; } = string.Empty;
}
