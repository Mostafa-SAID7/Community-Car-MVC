namespace CommunityCar.Domain.Policies.Account.Core;

/// <summary>
/// Core types and enums used across account policies
/// </summary>

public enum PasswordStrength
{
    VeryWeak = 0,
    Weak = 1,
    Fair = 2,
    Good = 3,
    Strong = 4,
    VeryStrong = 5
}

public enum AdminOperation
{
    ViewUsers = 1,
    EditUsers = 2,
    DeleteUsers = 3,
    ViewSystemLogs = 4,
    ManageRoles = 5,
    ManagePermissions = 6,
    SystemConfiguration = 7,
    DatabaseAccess = 8,
    SecuritySettings = 9,
    UnlockAccounts = 10
}