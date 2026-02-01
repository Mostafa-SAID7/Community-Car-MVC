namespace CommunityCar.Application.Features.Dashboard.ViewModels.Cache;

public class CacheManagementViewModel
{
    public bool IsRedisConnected { get; set; }
    public string CacheType { get; set; } = "Unknown";
    public Dictionary<string, object> Statistics { get; set; } = new();
}