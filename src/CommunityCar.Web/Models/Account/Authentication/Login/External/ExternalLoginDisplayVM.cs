namespace CommunityCar.Web.Models.Account.Authentication.Login.External;

public class ExternalLoginDisplayVM
{
    public string LoginProvider { get; set; } = string.Empty;
    public string ProviderDisplayName { get; set; } = string.Empty;
    public string ProviderKey { get; set; } = string.Empty;
    public bool IsLinked { get; set; }
    public DateTime? LinkedAt { get; set; }
}