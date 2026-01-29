namespace CommunityCar.Web.Models.Account.Authentication.External;

public class LinkedAccountVM
{
    public string LoginProvider { get; set; } = string.Empty;
    public string ProviderDisplayName { get; set; } = string.Empty;
    public string ProviderKey { get; set; } = string.Empty;
    public DateTime LinkedAt { get; set; }
    public bool CanUnlink { get; set; } = true;
    public string? Email { get; set; }
}