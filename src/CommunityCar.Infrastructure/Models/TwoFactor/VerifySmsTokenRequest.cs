namespace CommunityCar.Infrastructure.Models.TwoFactor;

public class VerifySmsTokenRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
