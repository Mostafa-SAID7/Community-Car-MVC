using CommunityCar.Application.Features.Dashboard.System.ViewModels;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.System;

public interface ISystemManagementService
{
    Task<SystemHealthVM> GetSystemHealthAsync();
    Task<List<SystemServiceVM>> GetSystemServicesAsync();
    Task<bool> RestartServiceAsync(string serviceName);
    Task<bool> StopServiceAsync(string serviceName);
    Task<bool> StartServiceAsync(string serviceName);
    Task<SystemConfigurationVM> GetSystemConfigurationAsync();
    Task<bool> UpdateSystemConfigurationAsync(SystemConfigurationVM configuration);
    Task<List<EnvironmentVariableVM>> GetEnvironmentVariablesAsync();
    Task<bool> SetEnvironmentVariableAsync(string key, string value);
    Task<bool> DeleteEnvironmentVariableAsync(string key);
    Task<DatabaseStatusVM> GetDatabaseStatusAsync();
    Task<bool> BackupDatabaseAsync();
    Task<bool> RestoreDatabaseAsync(string backupPath);
    Task<List<DatabaseBackupVM>> GetDatabaseBackupsAsync();
    Task<bool> OptimizeDatabaseAsync();
    Task<ServerInfoVM> GetServerInfoAsync();
    Task<List<LogFileVM>> GetLogFilesAsync();
    Task<string> GetLogFileContentAsync(string fileName, int lines = 100);
    Task<bool> ClearLogFileAsync(string fileName);
    Task<bool> DownloadLogFileAsync(string fileName);
}