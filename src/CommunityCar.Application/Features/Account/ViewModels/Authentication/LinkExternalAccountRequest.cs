using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Account.ViewModels.Authentication;

public class LinkExternalAccountRequest
{
    public string UserId { get; set; } = string.Empty;
    public string ExternalToken { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string? ExternalId { get; set; }
    public string? Email { get; set; }
}