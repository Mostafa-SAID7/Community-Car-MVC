namespace CommunityCar.Application.Features.Shared.ViewModels;

public class SearchSuggestionVM
{
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Score { get; set; }
}