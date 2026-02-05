namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.developer.Maintenance.ViewModels;

public class MaintenanceSettingsVM
{
    public bool AutoBackup { get; set; }
    public int BackupRetentionDays { get; set; }
    public bool AutoCleanupLogs { get; set; }
    public int LogRetentionDays { get; set; }
    public bool AutoOptimizeDatabase { get; set; }
    public string MaintenanceWindow { get; set; } = string.Empty;
}




