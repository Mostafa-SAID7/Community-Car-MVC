namespace CommunityCar.Application.Features.Dashboard.System.ViewModels;

public class DatabaseStatusVM
{
    public string Provider { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty; // Masked for security
    public bool IsConnected { get; set; }
    public DateTime LastChecked { get; set; }
    public long DatabaseSize { get; set; }
    public int TableCount { get; set; }
    public int RecordCount { get; set; }
    public double ResponseTime { get; set; }
    public List<DatabaseTableVM> Tables { get; set; } = new();
}