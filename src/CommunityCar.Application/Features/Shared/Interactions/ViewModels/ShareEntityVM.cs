using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Shared.Interactions.ViewModels;

public class ShareEntityVM
{
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public Guid UserId { get; set; }
    public ShareType ShareType { get; set; }
    public string? ShareMessage { get; set; }
    public string? Platform { get; set; }
}