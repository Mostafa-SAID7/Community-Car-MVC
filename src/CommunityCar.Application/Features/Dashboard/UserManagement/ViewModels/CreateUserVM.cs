namespace CommunityCar.Application.Features.Dashboard.UserManagement.ViewModels;

/// <summary>
/// ViewModel for creating a new user
/// </summary>
public class CreateUserVM
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public bool EmailConfirmed { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Location { get; set; }
}