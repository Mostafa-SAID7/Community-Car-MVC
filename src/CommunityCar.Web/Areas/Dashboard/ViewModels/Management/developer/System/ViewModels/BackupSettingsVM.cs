namespace CommunityCar.Application.Features.Dashboard.Settings.ViewModels;

public class BackupSettingsVM
{
    public bool EnableAutoBackup { get; set; }
    public string BackupFrequency { get; set; } = string.Empty;
    public int RetentionDays { get; set; }
    public string BackupLocation { get; set; } = string.Empty;
    public bool IncludeDatabase { get; set; }
    public bool IncludeFiles { get; set; }
    public bool CompressBackups { get; set; }
    public bool EncryptBackups { get; set; }
}