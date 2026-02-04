using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Account.ViewModels.Management;

public class UpdateUserVM
{
    [Required]
    [StringLength(50)]
    public string UserName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Bio { get; set; }
    
    public string? Avatar { get; set; }
    public bool IsActive { get; set; } = true;
    public List<string> Roles { get; set; } = new();
    public Dictionary<string, object> Profile { get; set; } = new();
}