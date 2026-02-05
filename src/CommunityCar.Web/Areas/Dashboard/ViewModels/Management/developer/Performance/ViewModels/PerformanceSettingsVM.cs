namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.developer.Performance.ViewModels;

public class PerformanceSettingsVM
{
    public bool EnableCaching { get; set; }
    public int CacheExpirationMinutes { get; set; }
    public bool EnableCompression { get; set; }
    public bool EnableMinification { get; set; }
    public bool EnableCdn { get; set; }
    public string CdnUrl { get; set; } = string.Empty;
    public int MaxRequestSize { get; set; }
    public int RequestTimeout { get; set; }
}




