namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class ManagementDashboardVM
{
    public Guid ManagerId { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    public int TotalManagedUsers { get; set; }
    public int DirectReports { get; set; }
    public int IndirectReports { get; set; }
    public List<ManagedUserVM> DirectManagedUsers { get; set; } = new();
    public List<ManagementHierarchyVM> Hierarchy { get; set; } = new();
    public Dictionary<string, int> ManagementStatistics { get; set; } = new();
}