namespace CommunityCar.Application.Features.Account.ViewModels.Authentication;

public class VerifyRecoveryCodeRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
}