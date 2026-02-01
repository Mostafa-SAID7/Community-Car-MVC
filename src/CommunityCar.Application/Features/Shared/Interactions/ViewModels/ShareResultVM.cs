namespace CommunityCar.Application.Features.Shared.Interactions.ViewModels;

public class ShareResultVM
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ShareUrl { get; set; }
    public ShareSummaryVM? Summary { get; set; }
}