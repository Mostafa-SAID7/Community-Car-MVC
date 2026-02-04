namespace CommunityCar.Application.Features.Dashboard.System.ViewModels;

public class DatabaseStatusVM
{
    public string Provider { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty; // Masked for security
    public string ServerName { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public DateTime LastChecked { get; set; }
    public long DatabaseSize { get; set; }
    public long Size { get; set; }
    public int TableCount { get; set; }
    public int RecordCount { get; set; }
    public int ActiveConnections { get; set; }
    public int MaxConnections { get; set; }
    public DateTime? LastBackup { get; set; }
    public double FragmentationLevel { get; set; }
    public bool IndexOptimizationNeeded { get; set; }
    public long TransactionLogSize { get; set; }
    public long FreeSpace { get; set; }
    public double ResponseTime { get; set; }
    public List<DatabaseTableVM> Tables { get; set; } = new();
}