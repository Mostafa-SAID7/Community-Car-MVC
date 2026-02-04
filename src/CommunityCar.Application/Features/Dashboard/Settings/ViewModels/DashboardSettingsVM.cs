using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Dashboard.Settings.ViewModels;

public class DashboardSettingsVM
{
    public Guid Id { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public string SiteDescription { get; set; } = string.Empty;
    public string SiteUrl { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string SupportEmail { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string FaviconUrl { get; set; } = string.Empty;
    public bool MaintenanceMode { get; set; }
    public string? MaintenanceMessage { get; set; }
    public bool AllowRegistration { get; set; }
    public bool RequireEmailConfirmation { get; set; }
    public bool EnableTwoFactorAuth { get; set; }
    public int SessionTimeout { get; set; }
    public int MaxLoginAttempts { get; set; }
    public int LockoutDuration { get; set; }
    public string DefaultLanguage { get; set; } = string.Empty;
    public string DefaultTimezone { get; set; } = string.Empty;
    public string DateFormat { get; set; } = string.Empty;
    public string TimeFormat { get; set; } = string.Empty;
    public bool EnableNotifications { get; set; }
    public bool EnableEmailNotifications { get; set; }
    public bool EnablePushNotifications { get; set; }
    public Dictionary<string, object> CustomSettings { get; set; } = new();
    public DateTime LastUpdated { get; set; }
    public string LastUpdatedBy { get; set; } = string.Empty;
}

public class SettingsCategoryVM
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<SettingItemVM> Settings { get; set; } = new();
}

public class SettingItemVM
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Text, Number, Boolean, Select, etc.
    public object? Value { get; set; }
    public object? DefaultValue { get; set; }
    public bool IsRequired { get; set; }
    public bool IsReadOnly { get; set; }
    public string? ValidationPattern { get; set; }
    public List<SettingOptionVM> Options { get; set; } = new();
    public Dictionary<string, object> Attributes { get; set; } = new();
}

public class SettingOptionVM
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}