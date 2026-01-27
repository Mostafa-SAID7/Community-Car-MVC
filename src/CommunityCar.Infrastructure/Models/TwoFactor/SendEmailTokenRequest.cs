namespace CommunityCar.Infrastructure.Models.TwoFactor;

public class SendEmailTokenRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Purpose { get; set; } = "TwoFactorAuthentication";
}
