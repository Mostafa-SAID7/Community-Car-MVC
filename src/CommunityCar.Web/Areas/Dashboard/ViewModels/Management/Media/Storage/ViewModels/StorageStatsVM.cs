namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Media.Storage.ViewModels;

public class StorageStatsVM
{
    public long TotalStorageBytes { get; set; }
    public long UsedStorageBytes { get; set; }
    public long AvailableStorageBytes { get; set; }
    public double UsagePercentage { get; set; }
    public Dictionary<string, long> StorageByType { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}




