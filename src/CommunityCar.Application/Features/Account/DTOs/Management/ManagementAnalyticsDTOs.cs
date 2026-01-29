namespace CommunityCar.Application.Features.Account.DTOs.Management;

#region Management Analytics DTOs

public class ManagementHierarchyDTO
{
    public Guid ManagerId { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    public List<UserManagementDTO> ManagedUsers { get; set; } = new();
    public int TotalManagedUsers { get; set; }
    public int DirectReports { get; set; }
}

public class ManagementAnalyticsDTO
{
    public Guid ManagerId { get; set; }
    public int TotalActions { get; set; }
    public Dictionary<string, int> ActionsByType { get; set; } = new();
    public DateTime? LastActionDate { get; set; }
    public List<UserManagementActionDTO> RecentActions { get; set; } = new();
}

public class ManagementStatsDTO
{
    public Guid ManagerId { get; set; }
    public int DirectReports { get; set; }
    public int IndirectReports { get; set; }
    public int TotalManagedUsers { get; set; }
    public int ActionsThisMonth { get; set; }
    public Dictionary<string, int> ActionBreakdown { get; set; } = new();
    public Dictionary<DateTime, int> ActionsByDate { get; set; } = new();
    public List<Guid> MostManagedUsers { get; set; } = new();
}

public class SystemManagementOverviewDTO
{
    public int TotalManagers { get; set; }
    public int TotalManagedUsers { get; set; }
    public int UnassignedUsers { get; set; }
    public int TotalActionsToday { get; set; }
    public Dictionary<string, int> ManagersByLevel { get; set; } = new();
    public List<Guid> TopManagers { get; set; } = new();
    public List<UserManagementActionDTO> RecentSystemActions { get; set; } = new();
}

#endregion