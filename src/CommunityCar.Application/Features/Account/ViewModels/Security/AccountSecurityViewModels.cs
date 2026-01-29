using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Account.ViewModels.Security;

public class SecurityOverviewVM
{
    public bool TwoFactorEnabled { get; set; }
    public int ActiveSessionsCount { get; set; }
    public DateTime? LastPasswordChange { get; set; }
    public bool HasExternalLogins { get; set; }
    public List<SecurityLogItemVM> RecentActivity { get; set; } = new();
}

public class SecurityLogItemVM
{
    public string Action { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Device { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public bool IsSuccess { get; set; }
}

public class ChangeEmailVM
{
    [Required]
    [EmailAddress]
    [Display(Name = "New Email")]
    public string NewEmail { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

public class SecurityVM
{
    public SecurityOverviewVM Overview { get; set; } = new();
    public List<UserSessionVM> ActiveSessions { get; set; } = new();
}

// Aliases for compatibility
public class UpdateEmailVM : ChangeEmailVM { }
public class SecurityOverviewWebVM : SecurityOverviewVM { }
public class SecurityLogItemWebVM : SecurityLogItemVM { }
