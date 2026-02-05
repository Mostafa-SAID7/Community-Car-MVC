namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.developer.System.ViewModels;

public class ServerInfoVM
{
    public string ServerName { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public int ProcessorCount { get; set; }
    public int TotalMemory { get; set; } // GB
    public int AvailableMemory { get; set; } // GB
    public string DotNetVersion { get; set; } = string.Empty;
    public DateTime ApplicationStartTime { get; set; }
    public int WorkingSet { get; set; } // MB
    public int GcTotalMemory { get; set; } // MB
    public int ThreadCount { get; set; }
    public int HandleCount { get; set; }
    public string TimeZone { get; set; } = string.Empty;
    public string CurrentDirectory { get; set; } = string.Empty;
}




