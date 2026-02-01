namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class ManagementHierarchyVM
{
    public Guid ManagerId { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    public string? ManagerProfilePicture { get; set; }
    public List<ManagedUserVM> ManagedUsers { get; set; } = new();
    public int TotalManagedUsers { get; set; }
    public int Level { get; set; }
}