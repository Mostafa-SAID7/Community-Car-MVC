using CommunityCar.Domain.Policies.Account.Security;

namespace CommunityCar.Domain.Policies.Account.Configuration;

public class AccountPolicyConfiguration
{
    public PasswordPolicySettings PasswordSettings { get; set; } = PasswordPolicySettings.Default;
    public LockoutPolicySettings LockoutSettings { get; set; } = LockoutPolicySettings.Default;
    public TimeSpan MfaSessionDuration { get; set; } = TimeSpan.FromHours(8);
    public bool EnableStrictMode { get; set; } = false;
    public bool EnableRelaxedMode { get; set; } = false;

    public static AccountPolicyConfiguration Default => new AccountPolicyConfiguration();

    public static AccountPolicyConfiguration Strict => new AccountPolicyConfiguration
    {
        PasswordSettings = PasswordPolicySettings.Strict,
        LockoutSettings = LockoutPolicySettings.Strict,
        MfaSessionDuration = TimeSpan.FromHours(4),
        EnableStrictMode = true
    };

    public static AccountPolicyConfiguration Relaxed => new AccountPolicyConfiguration
    {
        PasswordSettings = PasswordPolicySettings.Relaxed,
        LockoutSettings = LockoutPolicySettings.Relaxed,
        MfaSessionDuration = TimeSpan.FromHours(24),
        EnableRelaxedMode = true
    };
}