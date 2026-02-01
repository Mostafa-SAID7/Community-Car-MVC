namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class DeleteAccountRequest
{
    public Guid UserId { get; set; }
    public string Password { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}