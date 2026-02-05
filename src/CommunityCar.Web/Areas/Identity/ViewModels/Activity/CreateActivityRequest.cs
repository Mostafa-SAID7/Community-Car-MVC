namespace CommunityCar.Web.Areas.Identity.ViewModels.Activity;

public class CreateActivityRequest
{
    public Guid UserId { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
