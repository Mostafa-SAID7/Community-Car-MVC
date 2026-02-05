namespace CommunityCar.Application.Features.Shared.ViewModels.Users;

/// <summary>
/// Base user view model used across Dashboard and Account areas
/// </summary>
public class UserVM
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public bool IsOnline { get; set; }
    public string Status { get; set; } = string.Empty; // Active, Inactive, Locked, Banned
}

/// <summary>
/// Detailed user view model with additional information
/// </summary>
public class UserDetailVM : UserVM
{
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string? Website { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTime? LastPasswordChangeAt { get; set; }
    public int LoginCount { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
    
    // Admin-only fields (populated only in admin context)
    public string? LastIpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockedUntil { get; set; }
    public string? LockReason { get; set; }
    public int FailedLoginAttempts { get; set; }
    public List<UserSecurityEventVM> RecentSecurityEvents { get; set; } = new();
}

/// <summary>
/// Create user view model
/// </summary>
public class CreateUserVM
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public bool SendWelcomeEmail { get; set; } = true;
    public bool RequireEmailConfirmation { get; set; } = true;
    
    // Admin-only fields
    public bool IsAdminCreated { get; set; }
    public bool SkipEmailVerification { get; set; }
    public bool SetAsActive { get; set; } = true;
}

/// <summary>
/// Update user view model
/// </summary>
public class UpdateUserVM
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string? Website { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    
    // Admin-only fields
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public List<string>? Roles { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsVerified { get; set; }
}

/// <summary>
/// User activity view model
/// </summary>
public class UserActivityVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// User security event view model
/// </summary>
public class UserSecurityEventVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool IsResolved { get; set; }
    public string? Resolution { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}