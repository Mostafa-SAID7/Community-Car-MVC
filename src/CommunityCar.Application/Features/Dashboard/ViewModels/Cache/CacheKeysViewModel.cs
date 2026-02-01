namespace CommunityCar.Application.Features.Dashboard.ViewModels.Cache;

public class CacheKeysViewModel
{
    public string Pattern { get; set; } = "*";
    public List<string> Keys { get; set; } = new();
    public int TotalCount { get; set; }
}