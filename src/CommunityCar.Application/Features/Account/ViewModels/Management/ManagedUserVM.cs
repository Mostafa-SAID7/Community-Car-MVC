namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class ManagedUserVM
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