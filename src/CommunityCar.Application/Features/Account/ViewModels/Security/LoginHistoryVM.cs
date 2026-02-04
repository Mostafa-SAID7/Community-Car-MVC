using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Account.ViewModels.Security;

public class LoginHistoryVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; }
    public DateTime? LogoutTime { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Device { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
    public string? FailureReason { get; set; }
    public bool IsSuspicious { get; set; }
    public bool IsCurrentSession { get; set; }
    public TimeSpan? SessionDuration { get; set; }
}

public class SecurityEventVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty; // Login, Logout, PasswordChange, etc.
    public string Description { get; set; } = string.Empty;
    public DateTime EventTime { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
    public bool IsResolved { get; set; }
    public string? ResolutionNotes { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

public class DeviceVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty; // Mobile, Desktop, Tablet
    public string Browser { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime FirstSeen { get; set; }
    public DateTime LastSeen { get; set; }
    public bool IsTrusted { get; set; }
    public bool IsActive { get; set; }
    public int LoginCount { get; set; }
}

public class SecurityQuestionVM
{
    public Guid Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string? Answer { get; set; } // Only for updates, never returned
    public bool IsSet { get; set; }
    public DateTime? SetAt { get; set; }
    public DateTime? LastUsed { get; set; }
}

public class AccountLockoutVM
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool IsLockedOut { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public int FailedLoginAttempts { get; set; }
    public string? LockoutReason { get; set; }
    public DateTime? LastFailedLogin { get; set; }
    public string? LastFailedIpAddress { get; set; }
    public bool CanUnlock { get; set; }
}