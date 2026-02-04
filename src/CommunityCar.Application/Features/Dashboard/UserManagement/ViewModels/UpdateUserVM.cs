using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Dashboard.UserManagement.ViewModels;

/// <summary>
/// ViewModel for updating user information
/// </summary>
public class UpdateUserVM
{
    public Guid Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }

    public List<string> Roles { get; set; } = new();
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Location { get; set; }
}