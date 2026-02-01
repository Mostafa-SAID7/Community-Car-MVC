using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Shared.Interactions.ViewModels;

public class ShareVM
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserAvatar { get; set; }
    public ShareType ShareType { get; set; }
    public string ShareTypeDisplay { get; set; } = string.Empty;
    public string? ShareMessage { get; set; }
    public string? Platform { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
}