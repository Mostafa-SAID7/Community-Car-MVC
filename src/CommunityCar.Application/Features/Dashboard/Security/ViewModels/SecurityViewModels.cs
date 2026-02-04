namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class SecurityOverviewVM
{
    public int TotalThreats { get; set; }
    public int BlockedAttacks { get; set; }
    public int FailedLogins { get; set; }
    public int SuspiciousActivities { get; set; }
    public int SecurityScore { get; set; } // 0-100
    public DateTime LastSecurityScan { get; set; }
    public int TwoFactorEnabled { get; set; } // percentage
    public int PasswordStrengthAverage { get; set; } // percentage
}

public class SecurityThreatVM
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string AffectedEndpoint { get; set; } = string.Empty;
}

public class SecurityEventVM
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public bool Success { get; set; }
    public string Details { get; set; } = string.Empty;
}

public class FailedLoginAttemptVM
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime AttemptedAt { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}

public class SuspiciousActivityVM
{
    public Guid Id { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class SecurityAuditVM
{
    public DateTime LastAuditDate { get; set; }
    public int OverallScore { get; set; } // 0-100
    public int PassedChecks { get; set; }
    public int FailedChecks { get; set; }
    public List<string> Recommendations { get; set; } = new();
    public int VulnerabilitiesFound { get; set; }
    public int CriticalIssues { get; set; }
    public DateTime NextAuditDue { get; set; }
}

public class VulnerabilityVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AffectedComponent { get; set; } = string.Empty;
    public DateTime DiscoveredAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal CvssScore { get; set; }
    public string Recommendation { get; set; } = string.Empty;
}

public class BlockedIpVM
{
    public string IpAddress { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime BlockedAt { get; set; }
    public string BlockedBy { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}

public class SecuritySettingsVM
{
    public bool RequireTwoFactor { get; set; }
    public int PasswordMinLength { get; set; }
    public bool PasswordRequireUppercase { get; set; }
    public bool PasswordRequireLowercase { get; set; }
    public bool PasswordRequireNumbers { get; set; }
    public bool PasswordRequireSymbols { get; set; }
    public int MaxFailedLoginAttempts { get; set; }
    public int AccountLockoutDuration { get; set; } // minutes
    public int SessionTimeout { get; set; } // minutes
    public bool RequireEmailConfirmation { get; set; }
    public bool EnableSecurityHeaders { get; set; }
    public bool EnableRateLimiting { get; set; }
    public int RateLimitRequests { get; set; }
    public int RateLimitWindow { get; set; } // seconds
    public bool EnableIpBlocking { get; set; }
    public bool AutoBlockSuspiciousIps { get; set; }
}