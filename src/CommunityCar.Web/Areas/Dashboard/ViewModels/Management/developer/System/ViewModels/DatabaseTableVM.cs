namespace CommunityCar.Application.Features.Dashboard.Management.developer.System.ViewModels;

public class DatabaseTableVM
{
    public string Name { get; set; } = string.Empty;
    public int RecordCount { get; set; }
    public long Size { get; set; }
    public DateTime LastUpdated { get; set; }
}