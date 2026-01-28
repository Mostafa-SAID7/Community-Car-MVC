namespace CommunityCar.Application.Common.Models.Identity;

#region Base Models

public abstract class UserSummaryVM
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

#endregion

#region View Models

public class UserIdentityVM : UserSummaryVM
{
    public bool IsEmailConfirmed { get; set; }
    public bool IsLocked { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class RoleVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public string ConcurrencyStamp { get; set; } = string.Empty;
}

public class UserClaimVM
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

#endregion

#region Request Models

public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateRoleRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

#endregion
