namespace CommunityCar.Web.Models.Dashboard.System.Cache;

public class CacheKeysViewModel
{
    public string Pattern { get; set; } = "*";
    public List<string> Keys { get; set; } = new();
    public int TotalCount { get; set; }
}