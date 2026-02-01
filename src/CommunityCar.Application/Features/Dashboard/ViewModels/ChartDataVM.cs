namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class ChartDataVM
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public DateTime Date { get; set; }
    public string Color { get; set; } = string.Empty;
}