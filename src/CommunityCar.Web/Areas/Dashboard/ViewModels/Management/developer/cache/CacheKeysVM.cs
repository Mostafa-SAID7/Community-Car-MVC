namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.developer.cache;

public class CacheKeysVM
{
    public List<CacheKeyDetailVM> Keys { get; set; } = new();
    public int TotalCount { get; set; }
    public string SearchPattern { get; set; } = "*";
    public string Pattern { get; set; } = "*";
    public string FilterCategory { get; set; } = "All";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public int TotalPages { get; set; }
}




