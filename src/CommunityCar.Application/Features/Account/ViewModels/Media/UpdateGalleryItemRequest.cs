namespace CommunityCar.Application.Features.Account.ViewModels.Media;

public class UpdateGalleryItemRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Caption { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsPublic { get; set; }
}