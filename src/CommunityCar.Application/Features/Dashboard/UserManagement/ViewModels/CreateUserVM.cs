namespace CommunityCar.Application.Features.Dashboard.UserManagement.ViewModels;

public class CreateUserVM
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public bool SendWelcomeEmail { get; set; } = true;
    public bool RequireEmailConfirmation { get; set; } = true;
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? TimeZone { get; set; }
}