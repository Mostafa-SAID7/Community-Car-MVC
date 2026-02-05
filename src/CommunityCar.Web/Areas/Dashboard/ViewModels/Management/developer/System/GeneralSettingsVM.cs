namespace CommunityCar.Application.Features.Dashboard.Settings.ViewModels;

public class GeneralSettingsVM
{
    public string SiteName { get; set; } = string.Empty;
    public string SiteDescription { get; set; } = string.Empty;
    public string SiteUrl { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
    public string DefaultLanguage { get; set; } = string.Empty;
    public bool MaintenanceMode { get; set; }
    public string MaintenanceMessage { get; set; } = string.Empty;
}