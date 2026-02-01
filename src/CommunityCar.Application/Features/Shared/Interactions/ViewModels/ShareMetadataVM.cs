namespace CommunityCar.Application.Features.Shared.Interactions.ViewModels;

public class ShareMetadataVM
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Dictionary<string, string> SocialMediaUrls { get; set; } = new();
}