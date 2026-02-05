namespace CommunityCar.Application.Features.Dashboard.Settings.ViewModels;

public class SettingsCategoryVM
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<SettingItemVM> Settings { get; set; } = new();
}