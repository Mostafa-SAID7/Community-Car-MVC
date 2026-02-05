namespace CommunityCar.Application.Features.Dashboard.Settings.ViewModels;

public class SettingItemVM
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public object Value { get; set; } = new();
    public object DefaultValue { get; set; } = new();
    public bool IsRequired { get; set; }
    public string ValidationRules { get; set; } = string.Empty;
    public List<SettingOptionVM> Options { get; set; } = new();
}