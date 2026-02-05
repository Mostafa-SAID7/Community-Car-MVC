namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Users.Core;

/// <summary>
/// Admin create user view model for dashboard operations
/// </summary>
public class AdminCreateUserVM
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool SendWelcomeEmail { get; set; } = true;
}




