namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.developer.System.ViewModels;

public class LoggingSettingsVM
{
    public string LogLevel { get; set; } = string.Empty;
    public bool EnableFileLogging { get; set; }
    public bool EnableDatabaseLogging { get; set; }
    public bool EnableRemoteLogging { get; set; }
    public string RemoteLoggingUrl { get; set; } = string.Empty;
    public int MaxLogFileSize { get; set; }
    public int LogRetentionDays { get; set; }
}




