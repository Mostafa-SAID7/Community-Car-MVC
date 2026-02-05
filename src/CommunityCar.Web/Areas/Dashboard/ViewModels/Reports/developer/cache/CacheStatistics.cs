namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Reports.developer.cache;

public class CacheStatistics
{
    public long TotalKeys { get; set; }
    public long UsedMemory { get; set; }
    public long MaxMemory { get; set; }
    public double HitRate { get; set; }
    public double MissRate { get; set; }
    public int ConnectedClients { get; set; }
    public DateTime LastUpdated { get; set; }
}




