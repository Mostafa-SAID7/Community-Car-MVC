namespace CommunityCar.Application.Features.Dashboard.UserManagement.ViewModels;

public class UserManagementVM
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int LoginCount { get; set; }
    public int FailedLoginAttempts { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public string? ProfilePicture { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? TimeZone { get; set; }
}