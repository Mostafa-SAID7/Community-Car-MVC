using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Shared.ViewModels;

public class ChartDataVM
{
    public string Label { get; set; } = string.Empty;
    public double Value { get; set; }
    public DateTime Date { get; set; }
    public string Color { get; set; } = string.Empty;
}