using CommunityCar.Domain.Policies.Account.Configuration;
using CommunityCar.Domain.Policies.Account.Core;
using CommunityCar.Domain.Policies.Account.Authentication;
using CommunityCar.Domain.Policies.Account.Authorization;
using CommunityCar.Domain.Policies.Account.Security;

namespace CommunityCar.Domain.Policies.Account;

/// <summary>
/// Factory for creating and configuring account policies
/// </summary>
public class AccountPolicyFactory
{
    /// <summary>
    /// Creates a standard account policy manager with default settings
    /// </summary>
    public static AccountPolicyManager CreateStandard(
        Func<Guid, Task<bool>> isAdminChecker,
        Func<Guid, Task<IEnumerable<string>>> getUserRoles,
        Func<Guid, Task<IEnumerable<string>>> getUserPermissions,
        Func<Guid, Task<bool>> isMfaEnabledChecker,
        Func<Guid, Task<bool>> isMfaVerifiedChecker,
        Func<Guid, Task<DateTime?>> getLastMfaVerificationChecker,
        Func<Guid, Task<LockoutInfo>> getLockoutInfoChecker,
        Func<Guid, Task<IEnumerable<string>>> getPasswordHistoryChecker = null,
        Func<string, Task<bool>> isCommonPasswordChecker = null)
    {
        var adminPolicy = new RequireAdminPolicy(isAdminChecker, getUserRoles);
        var mfaPolicy = new RequireMfaPolicy(isMfaEnabledChecker, isMfaVerifiedChecker, getLastMfaVerificationChecker);
        var passwordPolicy = new PasswordPolicy(PasswordPolicySettings.Default, getPasswordHistoryChecker, isCommonPasswordChecker);
        var lockoutPolicy = new LockoutPolicy(LockoutPolicySettings.Default, getLockoutInfoChecker, getUserRoles);
        var mfaRequirementPolicy = new MfaRequirementPolicy(getUserRoles);
        var adminOperationPolicy = new AdminOperationPolicy(adminPolicy, getUserPermissions);

        return new AccountPolicyManager(
            adminPolicy,
            mfaPolicy,
            passwordPolicy,
            lockoutPolicy,
            mfaRequirementPolicy,
            adminOperationPolicy);
    }

    /// <summary>
    /// Creates a strict account policy manager with enhanced security settings
    /// </summary>
    public static AccountPolicyManager CreateStrict(
        Func<Guid, Task<bool>> isAdminChecker,
        Func<Guid, Task<IEnumerable<string>>> getUserRoles,
        Func<Guid, Task<IEnumerable<string>>> getUserPermissions,
        Func<Guid, Task<bool>> isMfaEnabledChecker,
        Func<Guid, Task<bool>> isMfaVerifiedChecker,
        Func<Guid, Task<DateTime?>> getLastMfaVerificationChecker,
        Func<Guid, Task<LockoutInfo>> getLockoutInfoChecker,
        Func<Guid, Task<IEnumerable<string>>> getPasswordHistoryChecker = null,
        Func<string, Task<bool>> isCommonPasswordChecker = null)
    {
        var adminPolicy = new RequireAdminPolicy(isAdminChecker, getUserRoles);
        var mfaPolicy = new RequireMfaPolicy(isMfaEnabledChecker, isMfaVerifiedChecker, getLastMfaVerificationChecker, TimeSpan.FromHours(4));
        var passwordPolicy = new PasswordPolicy(PasswordPolicySettings.Strict, getPasswordHistoryChecker, isCommonPasswordChecker);
        var lockoutPolicy = new LockoutPolicy(LockoutPolicySettings.Strict, getLockoutInfoChecker, getUserRoles);
        var mfaRequirementPolicy = new MfaRequirementPolicy(getUserRoles);
        var adminOperationPolicy = new AdminOperationPolicy(adminPolicy, getUserPermissions);

        return new AccountPolicyManager(
            adminPolicy,
            mfaPolicy,
            passwordPolicy,
            lockoutPolicy,
            mfaRequirementPolicy,
            adminOperationPolicy);
    }

    /// <summary>
    /// Creates a relaxed account policy manager with lenient settings
    /// </summary>
    public static AccountPolicyManager CreateRelaxed(
        Func<Guid, Task<bool>> isAdminChecker,
        Func<Guid, Task<IEnumerable<string>>> getUserRoles,
        Func<Guid, Task<IEnumerable<string>>> getUserPermissions,
        Func<Guid, Task<bool>> isMfaEnabledChecker,
        Func<Guid, Task<bool>> isMfaVerifiedChecker,
        Func<Guid, Task<DateTime?>> getLastMfaVerificationChecker,
        Func<Guid, Task<LockoutInfo>> getLockoutInfoChecker,
        Func<Guid, Task<IEnumerable<string>>> getPasswordHistoryChecker = null,
        Func<string, Task<bool>> isCommonPasswordChecker = null)
    {
        var adminPolicy = new RequireAdminPolicy(isAdminChecker, getUserRoles);
        var mfaPolicy = new RequireMfaPolicy(isMfaEnabledChecker, isMfaVerifiedChecker, getLastMfaVerificationChecker, TimeSpan.FromHours(24));
        var passwordPolicy = new PasswordPolicy(PasswordPolicySettings.Relaxed, getPasswordHistoryChecker, isCommonPasswordChecker);
        var lockoutPolicy = new LockoutPolicy(LockoutPolicySettings.Relaxed, getLockoutInfoChecker, getUserRoles);
        var mfaRequirementPolicy = new MfaRequirementPolicy(getUserRoles);
        var adminOperationPolicy = new AdminOperationPolicy(adminPolicy, getUserPermissions);

        return new AccountPolicyManager(
            adminPolicy,
            mfaPolicy,
            passwordPolicy,
            lockoutPolicy,
            mfaRequirementPolicy,
            adminOperationPolicy);
    }

    /// <summary>
    /// Creates a custom account policy manager with specified settings
    /// </summary>
    public static AccountPolicyManager CreateCustom(
        AccountPolicyConfiguration configuration,
        Func<Guid, Task<bool>> isAdminChecker,
        Func<Guid, Task<IEnumerable<string>>> getUserRoles,
        Func<Guid, Task<IEnumerable<string>>> getUserPermissions,
        Func<Guid, Task<bool>> isMfaEnabledChecker,
        Func<Guid, Task<bool>> isMfaVerifiedChecker,
        Func<Guid, Task<DateTime?>> getLastMfaVerificationChecker,
        Func<Guid, Task<LockoutInfo>> getLockoutInfoChecker,
        Func<Guid, Task<IEnumerable<string>>> getPasswordHistoryChecker = null,
        Func<string, Task<bool>> isCommonPasswordChecker = null)
    {
        var adminPolicy = new RequireAdminPolicy(isAdminChecker, getUserRoles);
        var mfaPolicy = new RequireMfaPolicy(isMfaEnabledChecker, isMfaVerifiedChecker, getLastMfaVerificationChecker, configuration.MfaSessionDuration);
        var passwordPolicy = new PasswordPolicy(configuration.PasswordSettings, getPasswordHistoryChecker, isCommonPasswordChecker);
        var lockoutPolicy = new LockoutPolicy(configuration.LockoutSettings, getLockoutInfoChecker, getUserRoles);
        var mfaRequirementPolicy = new MfaRequirementPolicy(getUserRoles);
        var adminOperationPolicy = new AdminOperationPolicy(adminPolicy, getUserPermissions);

        return new AccountPolicyManager(
            adminPolicy,
            mfaPolicy,
            passwordPolicy,
            lockoutPolicy,
            mfaRequirementPolicy,
            adminOperationPolicy);
    }

    /// <summary>
    /// Creates a policy manager with completely custom individual settings
    /// </summary>
    public static AccountPolicyManager CreateFullyCustom(
        PasswordPolicySettings passwordSettings,
        LockoutPolicySettings lockoutSettings,
        TimeSpan mfaSessionDuration,
        Func<Guid, Task<bool>> isAdminChecker,
        Func<Guid, Task<IEnumerable<string>>> getUserRoles,
        Func<Guid, Task<IEnumerable<string>>> getUserPermissions,
        Func<Guid, Task<bool>> isMfaEnabledChecker,
        Func<Guid, Task<bool>> isMfaVerifiedChecker,
        Func<Guid, Task<DateTime?>> getLastMfaVerificationChecker,
        Func<Guid, Task<LockoutInfo>> getLockoutInfoChecker,
        Func<Guid, Task<IEnumerable<string>>> getPasswordHistoryChecker = null,
        Func<string, Task<bool>> isCommonPasswordChecker = null)
    {
        var configuration = new AccountPolicyConfiguration
        {
            PasswordSettings = passwordSettings,
            LockoutSettings = lockoutSettings,
            MfaSessionDuration = mfaSessionDuration
        };

        return CreateCustom(
            configuration,
            isAdminChecker,
            getUserRoles,
            getUserPermissions,
            isMfaEnabledChecker,
            isMfaVerifiedChecker,
            getLastMfaVerificationChecker,
            getLockoutInfoChecker,
            getPasswordHistoryChecker,
            isCommonPasswordChecker);
    }
}