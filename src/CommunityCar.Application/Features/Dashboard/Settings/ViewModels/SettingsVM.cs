namespace CommunityCar.Application.Features.Dashboard.Settings.ViewModels;

/// <summary>
/// Generic settings view model for dashboard settings operations
/// </summary>
public class SettingsVM
{
    public string Key { get; set; } = string.Empty;
    public string SettingKey { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DataType { get; set; } = "string"; // string, int, bool, decimal
    public bool IsRequired { get; set; }
    public bool IsReadOnly { get; set; }
    public string? DefaultValue { get; set; }
    public string? ValidationPattern { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}