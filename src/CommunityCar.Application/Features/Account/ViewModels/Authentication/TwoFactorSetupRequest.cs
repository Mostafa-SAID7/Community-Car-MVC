namespace CommunityCar.Application.Features.Account.ViewModels.Authentication;

public class TwoFactorSetupRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
}