using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Common.Models.Account;

public class SecurityInfoVM
{
    public bool IsTwoFactorEnabled { get; set; }
    public DateTime? LastPasswordChange { get; set; }
    public int ActiveSessions { get; set; }
    public bool HasOAuthLinked { get; set; }
}

public class SecurityLogVM
{
    public string Action { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsSuccessful { get; set; }
    public string? Location { get; set; }
}

public class ActiveSessionVM
{
    public string SessionId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Location { get; set; } = "Unknown";
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public bool IsCurrent { get; set; }
}

public class TwoFactorSetupVM
{
    public string SecretKey { get; set; } = string.Empty;
    public string QrCodeUri { get; set; } = string.Empty;
    public List<string> RecoveryCodes { get; set; } = new();
}

public class TwoFactorSetupRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
}


