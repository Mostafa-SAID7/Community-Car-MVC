namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class AssignManagerRequest
{
    public Guid UserId { get; set; }
    public Guid ManagerId { get; set; }
    public string? Role { get; set; }
    public string? Reason { get; set; }
}