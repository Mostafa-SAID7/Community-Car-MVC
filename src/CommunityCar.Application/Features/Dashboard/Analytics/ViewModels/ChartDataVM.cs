namespace CommunityCar.Application.Features.Dashboard.Analytics.ViewModels;

public class ChartDataVM
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Color { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Category { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}