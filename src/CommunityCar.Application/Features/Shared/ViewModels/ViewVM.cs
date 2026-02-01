using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Shared.ViewModels;

public class ViewVM
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}