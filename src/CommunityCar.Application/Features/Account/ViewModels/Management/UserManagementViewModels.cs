namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class UserManagementVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ManagerId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string? UserProfilePicture { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public string AssignedTimeAgo { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
}

public class ManagementDashboardVM
{
    public Guid ManagerId { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    public int TotalManagedUsers { get; set; }
    public int DirectReports { get; set; }
    public int IndirectReports { get; set; }
    public List<UserManagementVM> DirectManagedUsers { get; set; } = new();
    public List<ManagementHierarchyVM> Hierarchy { get; set; } = new();
    public Dictionary<string, int> ManagementStatistics { get; set; } = new();
}

public class ManagementHierarchyVM
{
    public Guid ManagerId { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    public string? ManagerProfilePicture { get; set; }
    public List<UserManagementVM> ManagedUsers { get; set; } = new();
    public int TotalManagedUsers { get; set; }
    public int Level { get; set; }
}

public class AssignManagerVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid? CurrentManagerId { get; set; }
    public string? CurrentManagerName { get; set; }
    public List<ManagerOptionVM> AvailableManagers { get; set; } = new();
}

public class ManagerOptionVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public int ManagedUsersCount { get; set; }
    public bool IsAvailable { get; set; }
}