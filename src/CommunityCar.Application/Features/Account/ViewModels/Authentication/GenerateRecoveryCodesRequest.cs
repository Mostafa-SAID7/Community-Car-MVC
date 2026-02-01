namespace CommunityCar.Application.Features.Account.ViewModels.Authentication;

public class GenerateRecoveryCodesRequest
{
    public Guid UserId { get; set; }
    public string Password { get; set; } = string.Empty;
}