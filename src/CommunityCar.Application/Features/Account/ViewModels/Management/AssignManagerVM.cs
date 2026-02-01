namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class AssignManagerVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid? CurrentManagerId { get; set; }
    public string? CurrentManagerName { get; set; }
    public List<ManagerOptionVM> AvailableManagers { get; set; } = new();
}