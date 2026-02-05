using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Web.Areas.Identity.ViewModels.Authentication;

public class OAuthConnectionsVM
{
    public bool HasGoogleAccount { get; set; }
    public bool HasFacebookAccount { get; set; }
    public DateTime? GoogleLinkedAt { get; set; }
    public DateTime? FacebookLinkedAt { get; set; }
    public List<string> AvailableProviders { get; set; } = new();
}
