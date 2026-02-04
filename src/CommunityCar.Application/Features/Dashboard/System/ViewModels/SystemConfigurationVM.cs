namespace CommunityCar.Application.Features.Dashboard.System.ViewModels;

public class SystemConfigurationVM
{
    public string Environment { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string BuildNumber { get; set; } = string.Empty;
    public DateTime BuildDate { get; set; }
    public string DatabaseProvider { get; set; } = string.Empty;
    public string DatabaseVersion { get; set; } = string.Empty;
    public string CacheProvider { get; set; } = string.Empty;
    public string LoggingProvider { get; set; } = string.Empty;
    public bool MaintenanceMode { get; set; }
    public Dictionary<string, string> Settings { get; set; } = new();
    public List<EnvironmentVariableVM> EnvironmentVariables { get; set; } = new();
}