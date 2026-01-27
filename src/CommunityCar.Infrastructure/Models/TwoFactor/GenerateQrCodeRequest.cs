namespace CommunityCar.Infrastructure.Models.TwoFactor;

public class GenerateQrCodeRequest
{
    public string UserId { get; set; } = string.Empty;
    public string ApplicationName { get; set; } = "CommunityCar";
}
