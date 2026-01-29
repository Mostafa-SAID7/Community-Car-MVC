namespace CommunityCar.Application.Features.Account.DTOs.Management;

#region User Management DTOs

public class UserManagementDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ManagerId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public DateTime? RemovedAt { get; set; }
    public bool IsActive { get; set; }
}

public class AssignManagerRequest
{
    public Guid UserId { get; set; }
    public Guid ManagerId { get; set; }
    public string? Reason { get; set; }
}

public class RemoveManagerRequest
{
    public Guid UserId { get; set; }
    public string? Reason { get; set; }
}

public class TransferManagementRequest
{
    public Guid UserId { get; set; }
    public Guid NewManagerId { get; set; }
    public string? Reason { get; set; }
}

#endregion