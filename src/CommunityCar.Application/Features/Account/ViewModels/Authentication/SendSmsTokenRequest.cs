namespace CommunityCar.Application.Features.Account.ViewModels.Authentication;

public class SendSmsTokenRequest
{
    public Guid UserId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
}