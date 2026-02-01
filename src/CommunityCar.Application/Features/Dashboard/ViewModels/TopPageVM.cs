namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class TopPageVM
{
    public string Path { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Views { get; set; }
    public int UniqueViews { get; set; }
    public decimal BounceRate { get; set; }
}