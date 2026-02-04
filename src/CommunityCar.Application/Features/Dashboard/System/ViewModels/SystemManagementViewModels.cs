namespace CommunityCar.Application.Features.Dashboard.ViewModels;

public class SystemHealthVM
{
    public string OverallStatus { get; set; } = string.Empty;
    public decimal CpuUsage { get; set; }
    public decimal MemoryUsage { get; set; }
    public decimal DiskUsage { get; set; }
    public string DatabaseStatus { get; set; } = string.Empty;
    public string CacheStatus { get; set; } = string.Empty;
    public string QueueStatus { get; set; } = string.Empty;
    public DateTime LastHealthCheck { get; set; }
    public TimeSpan Uptime { get; set; }
    public int ActiveConnections { get; set; }
    public int RequestsPerMinute { get; set; }
    public decimal ErrorRate { get; set; }
    public int ResponseTime { get; set; }
}

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

public class SystemConfigurationVM
{
    public string ApplicationName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string DatabaseProvider { get; set; } = string.Empty;
    public string CacheProvider { get; set; } = string.Empty;
    public string LogLevel { get; set; } = string.Empty;
    public int MaxRequestSize { get; set; } // MB
    public int SessionTimeout { get; set; } // minutes
    public bool EnableDetailedErrors { get; set; }
    public bool EnableSwagger { get; set; }
    public bool EnableCors { get; set; }
    public List<string> AllowedOrigins { get; set; } = new();
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public bool EnableSsl { get; set; }
    public int MaxConcurrentRequests { get; set; }
    public int RequestTimeout { get; set; } // seconds
    public bool EnableCompression { get; set; }
    public bool EnableCaching { get; set; }
    public int CacheExpirationMinutes { get; set; }
}

public class EnvironmentVariableVM
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
}

public class DatabaseStatusVM
{
    public bool IsConnected { get; set; }
    public string ServerName { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public int Size { get; set; } // MB
    public int ActiveConnections { get; set; }
    public int MaxConnections { get; set; }
    public DateTime LastBackup { get; set; }
    public decimal FragmentationLevel { get; set; }
    public bool IndexOptimizationNeeded { get; set; }
    public int TransactionLogSize { get; set; } // MB
    public int FreeSpace { get; set; } // MB
}

public class DatabaseBackupVM
{
    public string FileName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int Size { get; set; } // MB
    public string Type { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public string Path { get; set; } = string.Empty;
}

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

public class LogFileVM
{
    public string FileName { get; set; } = string.Empty;
    public int Size { get; set; } // MB
    public DateTime CreatedAt { get; set; }
    public DateTime LastModified { get; set; }
    public int LineCount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}