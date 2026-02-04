using CommunityCar.Application.Common.Interfaces.Services.Dashboard.Security;
using CommunityCar.Application.Features.Dashboard.ViewModels;

namespace CommunityCar.Application.Services.Dashboard.Security;

public class SecurityService : ISecurityService
{
    public async Task<SecurityOverviewVM> GetSecurityOverviewAsync()
    {
        var random = new Random();
        
        return new SecurityOverviewVM
        {
            TotalThreats = random.Next(5, 25),
            BlockedAttacks = random.Next(50, 200),
            FailedLogins = random.Next(10, 100),
            SuspiciousActivities = random.Next(3, 15),
            SecurityScore = random.Next(75, 95),
            LastSecurityScan = DateTime.UtcNow.AddHours(-random.Next(1, 24)),
            TwoFactorEnabled = random.Next(60, 85), // percentage
            PasswordStrengthAverage = random.Next(70, 90) // percentage
        };
    }

    public async Task<List<SecurityThreatVM>> GetSecurityThreatsAsync(int limit = 50)
    {
        var threats = new List<SecurityThreatVM>();
        var random = new Random();
        var threatTypes = new[] { "SQL Injection", "XSS", "CSRF", "Brute Force", "DDoS", "Malware" };
        var severities = new[] { "Low", "Medium", "High", "Critical" };

        for (int i = 0; i < Math.Min(limit, 20); i++)
        {
            threats.Add(new SecurityThreatVM
            {
                Id = Guid.NewGuid(),
                Type = threatTypes[random.Next(threatTypes.Length)],
                Severity = severities[random.Next(severities.Length)],
                Source = $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}",
                Description = "Suspicious activity detected",
                DetectedAt = DateTime.UtcNow.AddMinutes(-random.Next(1, 1440)),
                Status = random.Next(2) == 0 ? "Active" : "Resolved",
                AffectedEndpoint = $"/api/{new[] { "users", "posts", "auth", "admin" }[random.Next(4)]}"
            });
        }

        return await Task.FromResult(threats);
    }

    public async Task<List<SecurityEventVM>> GetSecurityEventsAsync(DateTime startDate, DateTime endDate)
    {
        var events = new List<SecurityEventVM>();
        var random = new Random();
        var eventTypes = new[] { "Login", "Logout", "Password Change", "Permission Change", "Account Lock", "Failed Login" };

        var current = startDate;
        while (current <= endDate)
        {
            for (int i = 0; i < random.Next(5, 20); i++)
            {
                events.Add(new SecurityEventVM
                {
                    Id = Guid.NewGuid(),
                    EventType = eventTypes[random.Next(eventTypes.Length)],
                    UserId = Guid.NewGuid(),
                    UserName = $"user{random.Next(1, 1000)}",
                    IpAddress = $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}",
                    Timestamp = current.AddMinutes(random.Next(0, 1440)),
                    Success = random.Next(10) > 2, // 80% success rate
                    Details = "Security event details"
                });
            }
            current = current.AddDays(1);
        }

        return await Task.FromResult(events.OrderByDescending(e => e.Timestamp).ToList());
    }

    public async Task<List<FailedLoginAttemptVM>> GetFailedLoginAttemptsAsync(int limit = 100)
    {
        var attempts = new List<FailedLoginAttemptVM>();
        var random = new Random();

        for (int i = 0; i < Math.Min(limit, 50); i++)
        {
            attempts.Add(new FailedLoginAttemptVM
            {
                Id = Guid.NewGuid(),
                Email = $"user{random.Next(1, 1000)}@example.com",
                IpAddress = $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}",
                AttemptedAt = DateTime.UtcNow.AddMinutes(-random.Next(1, 10080)), // Last week
                Reason = new[] { "Invalid Password", "Account Locked", "Invalid Email", "Too Many Attempts" }[random.Next(4)],
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                Location = new[] { "New York, US", "London, UK", "Tokyo, JP", "Unknown" }[random.Next(4)]
            });
        }

        return await Task.FromResult(attempts.OrderByDescending(a => a.AttemptedAt).ToList());
    }

    public async Task<List<SuspiciousActivityVM>> GetSuspiciousActivitiesAsync(int limit = 50)
    {
        var activities = new List<SuspiciousActivityVM>();
        var random = new Random();
        var activityTypes = new[] { "Multiple Failed Logins", "Unusual Location", "High Request Rate", "Admin Access Attempt" };

        for (int i = 0; i < Math.Min(limit, 20); i++)
        {
            activities.Add(new SuspiciousActivityVM
            {
                Id = Guid.NewGuid(),
                ActivityType = activityTypes[random.Next(activityTypes.Length)],
                UserId = random.Next(2) == 0 ? Guid.NewGuid() : null,
                UserName = random.Next(2) == 0 ? $"user{random.Next(1, 1000)}" : null,
                IpAddress = $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}",
                DetectedAt = DateTime.UtcNow.AddMinutes(-random.Next(1, 1440)),
                RiskLevel = new[] { "Low", "Medium", "High" }[random.Next(3)],
                Description = "Suspicious activity detected based on behavior patterns",
                Status = random.Next(3) == 0 ? "Investigating" : "Resolved"
            });
        }

        return await Task.FromResult(activities.OrderByDescending(a => a.DetectedAt).ToList());
    }

    public async Task<SecurityAuditVM> GetSecurityAuditAsync()
    {
        var random = new Random();
        
        return new SecurityAuditVM
        {
            LastAuditDate = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
            OverallScore = random.Next(75, 95),
            PassedChecks = random.Next(15, 25),
            FailedChecks = random.Next(0, 5),
            Recommendations = new List<string>
            {
                "Enable two-factor authentication for all admin accounts",
                "Update password policy to require stronger passwords",
                "Implement rate limiting on login endpoints",
                "Review and update user permissions regularly"
            },
            VulnerabilitiesFound = random.Next(0, 8),
            CriticalIssues = random.Next(0, 2),
            NextAuditDue = DateTime.UtcNow.AddDays(random.Next(30, 90))
        };
    }

    public async Task<List<VulnerabilityVM>> GetVulnerabilitiesAsync()
    {
        var vulnerabilities = new List<VulnerabilityVM>();
        var random = new Random();
        var vulnTypes = new[] { "SQL Injection", "XSS", "CSRF", "Insecure Direct Object Reference", "Security Misconfiguration" };
        var severities = new[] { "Low", "Medium", "High", "Critical" };

        for (int i = 0; i < random.Next(3, 10); i++)
        {
            vulnerabilities.Add(new VulnerabilityVM
            {
                Id = Guid.NewGuid(),
                Title = $"{vulnTypes[random.Next(vulnTypes.Length)]} Vulnerability",
                Type = vulnTypes[random.Next(vulnTypes.Length)],
                Severity = severities[random.Next(severities.Length)],
                Description = "Vulnerability description and impact assessment",
                AffectedComponent = $"Component {random.Next(1, 10)}",
                DiscoveredAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                Status = new[] { "Open", "In Progress", "Resolved", "Accepted Risk" }[random.Next(4)],
                CvssScore = (decimal)(random.NextDouble() * 10),
                Recommendation = "Apply security patch or implement workaround"
            });
        }

        return await Task.FromResult(vulnerabilities.OrderByDescending(v => v.CvssScore).ToList());
    }

    public async Task<bool> BlockIpAddressAsync(string ipAddress, string reason)
    {
        // In real implementation, add IP to blocked list in database/cache
        await Task.Delay(100);
        return true;
    }

    public async Task<bool> UnblockIpAddressAsync(string ipAddress)
    {
        // In real implementation, remove IP from blocked list
        await Task.Delay(100);
        return true;
    }

    public async Task<List<BlockedIpVM>> GetBlockedIpsAsync()
    {
        var blockedIps = new List<BlockedIpVM>();
        var random = new Random();

        for (int i = 0; i < random.Next(5, 20); i++)
        {
            blockedIps.Add(new BlockedIpVM
            {
                IpAddress = $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}",
                Reason = new[] { "Brute Force Attack", "Suspicious Activity", "Spam", "Manual Block" }[random.Next(4)],
                BlockedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                BlockedBy = $"admin{random.Next(1, 5)}",
                ExpiresAt = random.Next(2) == 0 ? DateTime.UtcNow.AddDays(random.Next(1, 30)) : null,
                IsActive = random.Next(10) > 2 // 80% active
            });
        }

        return await Task.FromResult(blockedIps.OrderByDescending(b => b.BlockedAt).ToList());
    }

    public async Task<bool> EnableTwoFactorForUserAsync(Guid userId)
    {
        // In real implementation, enable 2FA for user
        await Task.Delay(100);
        return true;
    }

    public async Task<bool> DisableTwoFactorForUserAsync(Guid userId)
    {
        // In real implementation, disable 2FA for user
        await Task.Delay(100);
        return true;
    }

    public async Task<SecuritySettingsVM> GetSecuritySettingsAsync()
    {
        return new SecuritySettingsVM
        {
            RequireTwoFactor = true,
            PasswordMinLength = 8,
            PasswordRequireUppercase = true,
            PasswordRequireLowercase = true,
            PasswordRequireNumbers = true,
            PasswordRequireSymbols = true,
            MaxFailedLoginAttempts = 5,
            AccountLockoutDuration = 30, // minutes
            SessionTimeout = 120, // minutes
            RequireEmailConfirmation = true,
            EnableSecurityHeaders = true,
            EnableRateLimiting = true,
            RateLimitRequests = 100,
            RateLimitWindow = 60, // seconds
            EnableIpBlocking = true,
            AutoBlockSuspiciousIps = true
        };
    }

    public async Task<bool> UpdateSecuritySettingsAsync(SecuritySettingsVM settings)
    {
        // In real implementation, save settings to database
        await Task.Delay(100);
        return true;
    }

    public async Task<List<ChartDataVM>> GetSecurityMetricsChartAsync(int days)
    {
        var data = new List<ChartDataVM>();
        var random = new Random();
        var startDate = DateTime.UtcNow.AddDays(-days);

        for (int i = 0; i < days; i++)
        {
            var date = startDate.AddDays(i);
            data.Add(new ChartDataVM
            {
                Label = date.ToString("MMM dd"),
                Value = random.Next(0, 50), // Security events per day
                Date = date
            });
        }

        return await Task.FromResult(data);
    }
}