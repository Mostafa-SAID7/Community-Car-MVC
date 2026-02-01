using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Shared.ViewModels;

public class BookmarkVM
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public Guid UserId { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public string EntityTitle { get; set; } = string.Empty;
    public string EntityUrl { get; set; } = string.Empty;
}