namespace CommunityCar.Application.Features.Shared.ViewModels;

public class ActionButtonVM
{
    public string Type { get; set; } = string.Empty; // like, comment, share, bookmark, view
    public string Action { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
    public string OnClick { get; set; } = string.Empty;
}