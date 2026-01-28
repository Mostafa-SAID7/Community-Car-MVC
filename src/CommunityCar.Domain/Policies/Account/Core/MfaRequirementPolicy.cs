namespace CommunityCar.Domain.Policies.Account.Core;

/// <summary>
/// Policy for different MFA requirements based on user roles and actions
/// </summary>
public class MfaRequirementPolicy
{
    private readonly Func<Guid, Task<IEnumerable<string>>> _getUserRoles;
    private readonly Dictionary<string, MfaRequirement> _roleMfaRequirements;
    private readonly Dictionary<string, MfaRequirement> _actionMfaRequirements;

    public MfaRequirementPolicy(
        Func<Guid, Task<IEnumerable<string>>> getUserRoles)
    {
        _getUserRoles = getUserRoles ?? throw new ArgumentNullException(nameof(getUserRoles));
        _roleMfaRequirements = new Dictionary<string, MfaRequirement>(StringComparer.OrdinalIgnoreCase)
        {
            { "Admin", MfaRequirement.Required },
            { "SuperAdmin", MfaRequirement.Enforced },
            { "SystemAdmin", MfaRequirement.Enforced },
            { "Moderator", MfaRequirement.Recommended },
            { "User", MfaRequirement.Optional }
        };

        _actionMfaRequirements = new Dictionary<string, MfaRequirement>(StringComparer.OrdinalIgnoreCase)
        {
            { "ChangePassword", MfaRequirement.Required },
            { "ChangeEmail", MfaRequirement.Required },
            { "DeleteAccount", MfaRequirement.Enforced },
            { "ViewSensitiveData", MfaRequirement.Required },
            { "AdminAction", MfaRequirement.Enforced },
            { "FinancialTransaction", MfaRequirement.Enforced }
        };
    }

    public async Task<MfaRequirement> GetMfaRequirementAsync(Guid userId, string action = null)
    {
        var userRoles = await _getUserRoles(userId);
        var highestRoleRequirement = MfaRequirement.Optional;

        // Check role-based requirements
        foreach (var role in userRoles)
        {
            if (_roleMfaRequirements.TryGetValue(role, out var roleRequirement))
            {
                if (roleRequirement > highestRoleRequirement)
                    highestRoleRequirement = roleRequirement;
            }
        }

        // Check action-based requirements
        if (!string.IsNullOrEmpty(action) && 
            _actionMfaRequirements.TryGetValue(action, out var actionRequirement))
        {
            return actionRequirement > highestRoleRequirement ? actionRequirement : highestRoleRequirement;
        }

        return highestRoleRequirement;
    }
}

public enum MfaRequirement
{
    Optional = 0,      // MFA is optional
    Recommended = 1,   // MFA is recommended but not required
    Required = 2,      // MFA is required for access
    Enforced = 3       // MFA is strictly enforced, no exceptions
}