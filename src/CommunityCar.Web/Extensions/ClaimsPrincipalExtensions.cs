using System.Security.Claims;

namespace CommunityCar.Web.Extensions;

/// <summary>
/// Extension methods for ClaimsPrincipal to simplify user context access
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user ID from claims
    /// </summary>
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) 
                         ?? user.FindFirst("sub") 
                         ?? user.FindFirst("id");
        
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            return userId;
        
        return null;
    }

    /// <summary>
    /// Gets the user name from claims
    /// </summary>
    public static string? GetUserName(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Name)?.Value 
               ?? user.FindFirst("name")?.Value
               ?? user.FindFirst("preferred_username")?.Value;
    }

    /// <summary>
    /// Gets the user email from claims
    /// </summary>
    public static string? GetUserEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value 
               ?? user.FindFirst("email")?.Value;
    }

    /// <summary>
    /// Gets the user's full name from claims
    /// </summary>
    public static string? GetFullName(this ClaimsPrincipal user)
    {
        return user.FindFirst("full_name")?.Value 
               ?? user.FindFirst(ClaimTypes.GivenName)?.Value + " " + user.FindFirst(ClaimTypes.Surname)?.Value;
    }

    /// <summary>
    /// Checks if user is authenticated
    /// </summary>
    public static bool IsAuthenticated(this ClaimsPrincipal user)
    {
        return user.Identity?.IsAuthenticated == true;
    }

    /// <summary>
    /// Gets all user roles from claims
    /// </summary>
    public static IEnumerable<string> GetRoles(this ClaimsPrincipal user)
    {
        return user.FindAll(ClaimTypes.Role).Select(c => c.Value);
    }

    /// <summary>
    /// Checks if user has a specific role
    /// </summary>
    public static bool HasRole(this ClaimsPrincipal user, string role)
    {
        return user.IsInRole(role);
    }

    /// <summary>
    /// Checks if user has any of the specified roles
    /// </summary>
    public static bool HasAnyRole(this ClaimsPrincipal user, params string[] roles)
    {
        return roles.Any(role => user.IsInRole(role));
    }

    /// <summary>
    /// Gets a specific claim value
    /// </summary>
    public static string? GetClaimValue(this ClaimsPrincipal user, string claimType)
    {
        return user.FindFirst(claimType)?.Value;
    }

    /// <summary>
    /// Gets all values for a specific claim type
    /// </summary>
    public static IEnumerable<string> GetClaimValues(this ClaimsPrincipal user, string claimType)
    {
        return user.FindAll(claimType).Select(c => c.Value);
    }
}