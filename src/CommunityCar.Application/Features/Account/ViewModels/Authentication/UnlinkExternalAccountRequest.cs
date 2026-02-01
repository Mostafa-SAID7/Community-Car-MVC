using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Account.ViewModels.Authentication;

public class UnlinkExternalAccountRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
}