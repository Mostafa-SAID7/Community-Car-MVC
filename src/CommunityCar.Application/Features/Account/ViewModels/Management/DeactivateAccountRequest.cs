namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class DeactivateAccountRequest
{
    public Guid UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}