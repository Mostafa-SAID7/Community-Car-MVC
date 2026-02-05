namespace CommunityCar.Application.Features.Dashboard.Management.Users.Actions;

/// <summary>
/// User management history view model
/// </summary>
public class UserManagementHistoryVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}