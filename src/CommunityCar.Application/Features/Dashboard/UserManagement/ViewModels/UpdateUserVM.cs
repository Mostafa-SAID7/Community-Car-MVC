namespace CommunityCar.Application.Features.Dashboard.UserManagement.ViewModels;

public class UpdateUserVM
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? TimeZone { get; set; }
}