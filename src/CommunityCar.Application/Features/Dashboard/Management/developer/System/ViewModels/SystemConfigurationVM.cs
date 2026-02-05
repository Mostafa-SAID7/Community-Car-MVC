namespace CommunityCar.Application.Features.Dashboard.Management.developer.System.ViewModels;

public class SystemConfigurationVM
{
    public string ApplicationName { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string BuildNumber { get; set; } = string.Empty;
    public DateTime BuildDate { get; set; }
    public string DatabaseProvider { get; set; } = string.Empty;
    public string DatabaseVersion { get; set; } = string.Empty;
    public string CacheProvider { get; set; } = string.Empty;
    public string LoggingProvider { get; set; } = string.Empty;
    public string LogLevel { get; set; } = string.Empty;
    public int MaxRequestSize { get; set; }
    public int SessionTimeout { get; set; }
    public bool EnableDetailedErrors { get; set; }
    public bool EnableSwagger { get; set; }
    public bool EnableCors { get; set; }
    public List<string> AllowedOrigins { get; set; } = new();
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public bool EnableSsl { get; set; }
    public int MaxConcurrentRequests { get; set; }
    public int RequestTimeout { get; set; }
    public bool EnableCompression { get; set; }
    public bool EnableCaching { get; set; }
    public int CacheExpirationMinutes { get; set; }
    public bool MaintenanceMode { get; set; }
    public Dictionary<string, string> Settings { get; set; } = new();
    public List<EnvironmentVariableVM> EnvironmentVariables { get; set; } = new();
}