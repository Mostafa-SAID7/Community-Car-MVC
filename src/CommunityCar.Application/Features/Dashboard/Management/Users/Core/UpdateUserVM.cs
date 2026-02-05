namespace CommunityCar.Application.Features.Dashboard.Management.Users.Core;

/// <summary>
/// Update user view model
/// </summary>
public class UpdateUserVM
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public List<string> Roles { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
}