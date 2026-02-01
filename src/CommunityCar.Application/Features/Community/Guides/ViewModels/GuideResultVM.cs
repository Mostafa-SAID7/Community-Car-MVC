namespace CommunityCar.Application.Features.Community.Guides.ViewModels;

public class GuideResultVM
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? GuideId { get; set; }
}