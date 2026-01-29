namespace CommunityCar.Web.Models.Account.Authentication;

public class OAuthConnectionsWebVM
{
    public bool HasGoogleAccount { get; set; }
    public bool HasFacebookAccount { get; set; }
    public DateTime? GoogleLinkedAt { get; set; }
    public DateTime? FacebookLinkedAt { get; set; }
    public List<string> AvailableProviders { get; set; } = new();
}

public class LinkAccountWebVM
{
    public string Provider { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
}

public class UnlinkAccountWebVM
{
    public string Provider { get; set; } = string.Empty;
    public bool ConfirmUnlink { get; set; }
}