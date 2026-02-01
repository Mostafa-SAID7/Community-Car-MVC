namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class ContentModerationVM
{
    public List<ModerationItemVM> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PendingCount { get; set; }
    public int ApprovedCount { get; set; }
    public int RejectedCount { get; set; }
}