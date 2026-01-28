using CommunityCar.Domain.Base;
using System.Text.Json;

namespace CommunityCar.Domain.Entities.Dashboard.Settings;

public class DashboardSettings : BaseEntity
{
    public Guid UserId { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public string SettingType { get; set; } = string.Empty; // String, Number, Boolean, JSON
    public string Category { get; set; } = string.Empty; // Display, Notifications, Security, etc.
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
    public bool IsSystemSetting { get; set; }

    public DashboardSettings()
    {
        IsDefault = false;
        IsSystemSetting = false;
    }

    public void UpdateValue(string newValue)
    {
        SettingValue = newValue;
        Audit(UpdatedBy);
    }

    public T GetValue<T>()
    {
        return SettingType switch
        {
            "Boolean" => (T)(object)bool.Parse(SettingValue),
            "Number" => (T)(object)decimal.Parse(SettingValue),
            "JSON" => JsonSerializer.Deserialize<T>(SettingValue)!,
            _ => (T)(object)SettingValue
        };
    }
}