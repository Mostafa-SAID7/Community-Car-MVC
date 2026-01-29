namespace CommunityCar.Application.Features.Account.DTOs.Management;

public class UserManagementActionDTO
{
    public Guid Id { get; set; }
    public Guid ManagerId { get; set; }
    public Guid TargetUserId { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    public string TargetUserName { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LogManagementActionRequest
{
    public Guid ManagerId { get; set; }
    public Guid TargetUserId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class ManagementActionSearchRequest
{
    public Guid? ManagerId { get; set; }
    public Guid? TargetUserId { get; set; }
    public string? ActionType { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}