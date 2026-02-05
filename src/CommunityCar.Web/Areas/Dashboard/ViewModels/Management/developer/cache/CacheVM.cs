using CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.developer.cache;

namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.developer.cache;

public class CacheVM
{
    public CacheStatistics Statistics { get; set; } = new();
    public List<CacheKeyInfo> Keys { get; set; } = new();
    public bool IsRedisConnected { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
}




