namespace CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

public class AnalyticsVM
{
    public DateTime StartDate { get; set; } = DateTime.UtcNow.AddDays(-30);
    public DateTime EndDate { get; set; } = DateTime.UtcNow;
    public string? FilterType { get; set; }
    public string? FilterValue { get; set; }
    public int PageSize { get; set; } = 20;
    public int Page { get; set; } = 1;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "desc";
}