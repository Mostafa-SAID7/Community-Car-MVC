using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.ValueObjects.Account;

public class TwoFactorSettings : ValueObject
{
    public bool TwoFactorEnabled { get; private set; }
    public string? TwoFactorSecretKey { get; private set; }
    public DateTime? TwoFactorEnabledAt { get; private set; }
    public string? BackupCodes { get; private set; }
    public int FailedTwoFactorAttempts { get; private set; }
    public DateTime? TwoFactorLockoutEnd { get; private set; }
    public bool SmsEnabled { get; private set; }
    public bool EmailTwoFactorEnabled { get; private set; }

    // Parameterless constructor for EF Core
    private TwoFactorSettings()
    {
        TwoFactorEnabled = false;
        TwoFactorSecretKey = null;
        TwoFactorEnabledAt = null;
        BackupCodes = null;
        FailedTwoFactorAttempts = 0;
        TwoFactorLockoutEnd = null;
        SmsEnabled = false;
        EmailTwoFactorEnabled = false;
    }

    public TwoFactorSettings(
        bool twoFactorEnabled = false,
        string? secretKey = null,
        DateTime? enabledAt = null,
        string? backupCodes = null,
        int failedAttempts = 0,
        DateTime? lockoutEnd = null,
        bool smsEnabled = false,
        bool emailTwoFactorEnabled = false)
    {
        TwoFactorEnabled = twoFactorEnabled;
        TwoFactorSecretKey = secretKey;
        TwoFactorEnabledAt = enabledAt;
        BackupCodes = backupCodes;
        FailedTwoFactorAttempts = failedAttempts;
        TwoFactorLockoutEnd = lockoutEnd;
        SmsEnabled = smsEnabled;
        EmailTwoFactorEnabled = emailTwoFactorEnabled;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TwoFactorEnabled;
        yield return TwoFactorSecretKey ?? string.Empty;
        yield return TwoFactorEnabledAt ?? DateTime.MinValue;
        yield return BackupCodes ?? string.Empty;
        yield return FailedTwoFactorAttempts;
        yield return TwoFactorLockoutEnd ?? DateTime.MinValue;
        yield return SmsEnabled;
        yield return EmailTwoFactorEnabled;
    }

    public static TwoFactorSettings Disabled => new();

    public bool IsLockedOut => TwoFactorLockoutEnd.HasValue && TwoFactorLockoutEnd > DateTime.UtcNow;

    public TwoFactorSettings Enable(string secretKey)
    {
        return new TwoFactorSettings(
            twoFactorEnabled: true,
            secretKey: secretKey,
            enabledAt: DateTime.UtcNow,
            backupCodes: BackupCodes,
            failedAttempts: 0,
            lockoutEnd: null,
            smsEnabled: SmsEnabled,
            emailTwoFactorEnabled: EmailTwoFactorEnabled);
    }

    public TwoFactorSettings Disable()
    {
        return new TwoFactorSettings();
    }

    public TwoFactorSettings IncrementFailures()
    {
        var newFailedAttempts = FailedTwoFactorAttempts + 1;
        var newLockoutEnd = newFailedAttempts >= 5 ? DateTime.UtcNow.AddMinutes(15) : TwoFactorLockoutEnd;

        return new TwoFactorSettings(
            TwoFactorEnabled,
            TwoFactorSecretKey,
            TwoFactorEnabledAt,
            BackupCodes,
            newFailedAttempts,
            newLockoutEnd,
            SmsEnabled,
            EmailTwoFactorEnabled);
    }

    public TwoFactorSettings ResetFailures()
    {
        return new TwoFactorSettings(
            TwoFactorEnabled,
            TwoFactorSecretKey,
            TwoFactorEnabledAt,
            BackupCodes,
            0,
            null,
            SmsEnabled,
            EmailTwoFactorEnabled);
    }
}