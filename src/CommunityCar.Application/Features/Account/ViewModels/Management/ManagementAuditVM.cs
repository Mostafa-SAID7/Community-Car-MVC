namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class ManagementAuditVM
{
    public Guid UserId { get; set; }
    public List<UserManagementActionVM> Actions { get; set; } = new();
    public int TotalActions { get; set; }
    public Dictionary<string, int> ActionsByType { get; set; } = new();
    public DateTime? LastActionDate { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasMore { get; set; }
}