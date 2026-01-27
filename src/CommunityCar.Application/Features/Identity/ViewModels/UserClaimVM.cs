namespace CommunityCar.Application.Features.Identity.ViewModels;

/// <summary>
/// View model for user claim information
/// </summary>
public class UserClaimVM
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
}


