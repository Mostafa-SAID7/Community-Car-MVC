namespace CommunityCar.Application.Features.Account.DTOs.Authentication;

#region OAuth DTOs

public class OAuthInfoDTO
{
    public bool HasGoogleAccount { get; set; }
    public bool HasFacebookAccount { get; set; }
    public string? GoogleId { get; set; }
    public string? FacebookId { get; set; }
    public DateTime? GoogleLinkedAt { get; set; }
    public DateTime? FacebookLinkedAt { get; set; }
}

public class OAuthLinkRequest
{
    public Guid UserId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
}

public class OAuthUnlinkRequest
{
    public Guid UserId { get; set; }
    public string Provider { get; set; } = string.Empty;
}

public class OAuthProviderInfo
{
    public string Provider { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime LinkedAt { get; set; }
    public bool IsActive { get; set; }
}

#endregion