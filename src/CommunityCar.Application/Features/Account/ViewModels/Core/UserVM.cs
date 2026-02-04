namespace CommunityCar.Application.Features.Account.ViewModels.Core;

public class UserVM
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public bool IsLocked { get; set; }
    public List<string> Roles { get; set; } = new();
    public Dictionary<string, object> Profile { get; set; } = new();
}