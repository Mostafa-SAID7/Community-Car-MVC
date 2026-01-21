using System;
using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums;

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
        // Generate appropriate share URL based on entity type
        ShareUrl = EntityType switch
        {
            EntityType.Post => $"/posts/{EntityId}",
            EntityType.Question => $"/qa/{EntityId}",
            EntityType.Answer => $"/qa/{EntityId}#answer-{Id}",
            EntityType.Story => $"/stories/{EntityId}",
            EntityType.Review => $"/reviews/{EntityId}",
            EntityType.Event => $"/events/{EntityId}",
            EntityType.Guide => $"/guides/{EntityId}",
            EntityType.Group => $"/groups/{EntityId}",
            _ => $"/{EntityType.ToString().ToLower()}/{EntityId}"
        };
    }
}