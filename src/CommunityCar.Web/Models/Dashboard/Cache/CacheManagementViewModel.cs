namespace CommunityCar.Web.Models.Dashboard.Cache;

public class CacheManagementViewModel
{
    public bool IsRedisConnected { get; set; }
    public string CacheType { get; set; } = "Unknown";
    public Dictionary<string, object> Statistics { get; set; } = new();
}