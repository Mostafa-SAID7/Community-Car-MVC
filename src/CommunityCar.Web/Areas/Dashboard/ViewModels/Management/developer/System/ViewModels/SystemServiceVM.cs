namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.developer.System.ViewModels;

public class SystemServiceVM
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Port { get; set; }
    public int ProcessId { get; set; }
    public DateTime StartTime { get; set; }
    public int MemoryUsage { get; set; } // MB
    public decimal CpuUsage { get; set; }
    public string Description { get; set; } = string.Empty;
}




