using CommunityCar.Domain.Policies;
using CommunityCar.Domain.Policies.Account.Core;
using CommunityCar.Domain.Policies.Account.Authentication;
using CommunityCar.Domain.Policies.Account.Authorization;
using CommunityCar.Domain.Policies.Account.Security;

namespace CommunityCar.Domain.Policies.Account;

/// <summary>
/// Centralized manager for all account-related policies
/// </summary>
public class AccountPolicyManager
{
    private readonly RequireAdminPolicy _adminPolicy;
    private readonly RequireMfaPolicy _mfaPolicy;
    private readonly PasswordPolicy _passwordPolicy;
    private readonly LockoutPolicy _lockoutPolicy;
    private readonly MfaRequirementPolicy _mfaRequirementPolicy;
    private readonly AdminOperationPolicy _adminOperationPolicy;

    public AccountPolicyManager(
        RequireAdminPolicy adminPolicy,
        RequireMfaPolicy mfaPolicy,
        PasswordPolicy passwordPolicy,
        LockoutPolicy lockoutPolicy,
        MfaRequirementPolicy mfaRequirementPolicy,
        AdminOperationPolicy adminOperationPolicy)
    {
        _adminPolicy = adminPolicy ?? throw new ArgumentNullException(nameof(adminPolicy));
        _mfaPolicy = mfaPolicy ?? throw new ArgumentNullException(nameof(mfaPolicy));
        _passwordPolicy = passwordPolicy ?? throw new ArgumentNullException(nameof(passwordPolicy));
        _lockoutPolicy = lockoutPolicy ?? throw new ArgumentNullException(nameof(lockoutPolicy));
        _mfaRequirementPolicy = mfaRequirementPolicy ?? throw new ArgumentNullException(nameof(mfaRequirementPolicy));
        _adminOperationPolicy = adminOperationPolicy ?? throw new ArgumentNullException(nameof(adminOperationPolicy));
    }

    /// <summary>
    /// Evaluates all relevant policies for a user action
    /// </summary>
    public async Task<PolicyEvaluationResult> EvaluateUserActionAsync(UserActionRequest request)
    {
        var result = new PolicyEvaluationResult
        {
            UserId = request.UserId,
            Action = request.Action,
            IsAllowed = true
        };

        var evaluations = new List<PolicyEvaluation>();

        // Check lockout policy first
        var lockoutEvaluation = await EvaluateLockoutPolicyAsync(request);
        evaluations.Add(lockoutEvaluation);
        if (!lockoutEvaluation.IsAllowed)
        {
            result.IsAllowed = false;
            result.PrimaryReason = lockoutEvaluation.Reason;
        }

        // Check admin requirements
        if (request.RequiresAdmin)
        {
            var adminEvaluation = await EvaluateAdminPolicyAsync(request);
            evaluations.Add(adminEvaluation);
            if (!adminEvaluation.IsAllowed)
            {
                result.IsAllowed = false;
                if (string.IsNullOrEmpty(result.PrimaryReason))
                    result.PrimaryReason = adminEvaluation.Reason;
            }
        }

        // Check MFA requirements
        var mfaRequirement = await _mfaRequirementPolicy.GetMfaRequirementAsync(request.UserId, request.Action);
        if (mfaRequirement > MfaRequirement.Optional)
        {
            var mfaEvaluation = await EvaluateMfaPolicyAsync(request, mfaRequirement);
            evaluations.Add(mfaEvaluation);
            if (!mfaEvaluation.IsAllowed)
            {
                result.IsAllowed = false;
                if (string.IsNullOrEmpty(result.PrimaryReason))
                    result.PrimaryReason = mfaEvaluation.Reason;
            }
        }

        // Check password policy for password-related actions
        if (IsPasswordAction(request.Action))
        {
            var passwordEvaluation = await EvaluatePasswordPolicyAsync(request);
            evaluations.Add(passwordEvaluation);
            if (!passwordEvaluation.IsAllowed)
            {
                result.IsAllowed = false;
                if (string.IsNullOrEmpty(result.PrimaryReason))
                    result.PrimaryReason = passwordEvaluation.Reason;
            }
        }

        result.PolicyEvaluations = evaluations;
        return result;
    }

    /// <summary>
    /// Evaluates lockout policy
    /// </summary>
    private async Task<PolicyEvaluation> EvaluateLockoutPolicyAsync(UserActionRequest request)
    {
        var lockoutRequest = new LockoutRequest
        {
            UserId = request.UserId,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            Action = request.Action
        };

        var isAllowed = await _lockoutPolicy.CanAccessAsync(request.UserId, lockoutRequest);
        
        return new PolicyEvaluation
        {
            PolicyType = "Lockout",
            IsAllowed = isAllowed,
            Reason = isAllowed ? "Account is not locked" : "Account is currently locked"
        };
    }

    /// <summary>
    /// Evaluates admin policy
    /// </summary>
    private async Task<PolicyEvaluation> EvaluateAdminPolicyAsync(UserActionRequest request)
    {
        bool isAllowed;
        string reason;

        if (request.RequiredAdminOperation.HasValue)
        {
            isAllowed = await _adminOperationPolicy.CanAccessAsync(request.UserId, request.RequiredAdminOperation.Value);
            reason = isAllowed ? "User has required admin privileges" : "User lacks required admin privileges for this operation";
        }
        else
        {
            isAllowed = await _adminPolicy.CanAccessAsync(request.UserId, new object());
            reason = isAllowed ? "User has admin privileges" : "User lacks admin privileges";
        }

        return new PolicyEvaluation
        {
            PolicyType = "Admin",
            IsAllowed = isAllowed,
            Reason = reason
        };
    }

    /// <summary>
    /// Evaluates MFA policy
    /// </summary>
    private async Task<PolicyEvaluation> EvaluateMfaPolicyAsync(UserActionRequest request, MfaRequirement requirement)
    {
        var mfaResource = new MfaRequiredResource
        {
            ResourceName = request.Action,
            RequiresMfa = requirement >= MfaRequirement.Required,
            RequiresFreshMfa = IsHighSecurityAction(request.Action),
            RequirementLevel = requirement
        };

        var isAllowed = await _mfaPolicy.CanAccessAsync(request.UserId, mfaResource);
        var reason = isAllowed ? "MFA requirements satisfied" : GetMfaFailureReason(requirement);

        return new PolicyEvaluation
        {
            PolicyType = "MFA",
            IsAllowed = isAllowed,
            Reason = reason,
            RequirementLevel = requirement.ToString()
        };
    }

    /// <summary>
    /// Evaluates password policy
    /// </summary>
    private async Task<PolicyEvaluation> EvaluatePasswordPolicyAsync(UserActionRequest request)
    {
        var passwordRequest = new PasswordRequest
        {
            NewPassword = request.NewPassword,
            CurrentPassword = request.CurrentPassword
        };

        var isAllowed = await _passwordPolicy.CanAccessAsync(request.UserId, passwordRequest);
        var reason = isAllowed ? "Password meets policy requirements" : "Password does not meet policy requirements";

        return new PolicyEvaluation
        {
            PolicyType = "Password",
            IsAllowed = isAllowed,
            Reason = reason
        };
    }

    /// <summary>
    /// Gets detailed password validation results
    /// </summary>
    public async Task<PasswordValidationResult> ValidatePasswordAsync(Guid userId, string newPassword, string currentPassword = null)
    {
        return await _passwordPolicy.ValidatePasswordAsync(userId, newPassword, currentPassword);
    }

    /// <summary>
    /// Gets lockout decision for a user
    /// </summary>
    public async Task<LockoutDecision> EvaluateLockoutAsync(Guid userId, string ipAddress = null, string userAgent = null)
    {
        return await _lockoutPolicy.EvaluateLockoutAsync(userId, ipAddress, userAgent);
    }

    /// <summary>
    /// Unlocks a user account
    /// </summary>
    public async Task<UnlockResult> UnlockAccountAsync(Guid userId, Guid adminUserId, string reason)
    {
        return await _lockoutPolicy.UnlockAccountAsync(userId, adminUserId, reason);
    }

    private bool IsPasswordAction(string action)
    {
        var passwordActions = new[] { "ChangePassword", "ResetPassword", "SetPassword" };
        return passwordActions.Contains(action, StringComparer.OrdinalIgnoreCase);
    }

    private bool IsHighSecurityAction(string action)
    {
        var highSecurityActions = new[] { "DeleteAccount", "ChangeEmail", "AdminAction", "FinancialTransaction" };
        return highSecurityActions.Contains(action, StringComparer.OrdinalIgnoreCase);
    }

    private string GetMfaFailureReason(MfaRequirement requirement)
    {
        return requirement switch
        {
            MfaRequirement.Required => "Multi-factor authentication is required for this action",
            MfaRequirement.Enforced => "Multi-factor authentication is strictly required and cannot be bypassed",
            _ => "Multi-factor authentication verification failed"
        };
    }
}

/// <summary>
/// Request object for user action evaluation
/// </summary>
public class UserActionRequest
{
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public bool RequiresAdmin { get; set; }
    public AdminOperation? RequiredAdminOperation { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
}

/// <summary>
/// Result of policy evaluation
/// </summary>
public class PolicyEvaluationResult
{
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public bool IsAllowed { get; set; }
    public string PrimaryReason { get; set; } = string.Empty;
    public List<PolicyEvaluation> PolicyEvaluations { get; set; } = new List<PolicyEvaluation>();
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Individual policy evaluation result
/// </summary>
public class PolicyEvaluation
{
    public string PolicyType { get; set; } = string.Empty;
    public bool IsAllowed { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string RequirementLevel { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalInfo { get; set; } = new Dictionary<string, object>();
}