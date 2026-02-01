using System;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Domain.Entities.Shared;

public class Share : BaseEntity
{
    public Guid EntityId { get; private set; }
    public EntityType EntityType { get; private set; }
    public Guid UserId { get; private set; }
    public ShareType ShareType { get; private set; }
    public string? ShareMessage { get; private set; }
    public string? ShareUrl { get; private set; }
    public string? Platform { get; private set; } // Facebook, Twitter, WhatsApp, etc.

    public Share(Guid entityId, EntityType entityType, Guid userId, ShareType shareType, string? shareMessage = null, string? platform = null)
    {
        EntityId = entityId;
        EntityType = entityType;
        UserId = userId;
        ShareType = shareType;
        ShareMessage = shareMessage;
        Platform = platform;
        GenerateShareUrl();
    }

    private Share() { }

    public void UpdateShareMessage(string? message)
    {
        ShareMessage = message;
        Audit(UpdatedBy);
    }

    private void GenerateShareUrl()
    {
        // Generate appropriate share URL based on entity type using slugs where available
        ShareUrl = EntityType switch
        {
            EntityType.Post => $"/posts/{EntityId}", // Will be updated to use slug in controller
            EntityType.Question => $"/qa/{EntityId}", // Will be updated to use slug in controller
            EntityType.Answer => $"/qa/{EntityId}#answer-{Id}", // Will be updated to use slug in controller
            EntityType.Story => $"/stories/{EntityId}", // Will be updated to use slug in controller
            EntityType.Review => $"/reviews/{EntityId}",
            EntityType.Event => $"/events/{EntityId}", // Will be updated to use slug in controller
            EntityType.Guide => $"/guides/{EntityId}", // Will be updated to use slug in controller
            EntityType.Group => $"/groups/{EntityId}", // Will be updated to use slug in controller
            _ => $"/{EntityType.ToString().ToLower()}/{EntityId}"
        };
    }
}