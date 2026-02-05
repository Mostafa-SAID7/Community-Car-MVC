namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Media.Localization.ViewModels;

public class CultureVM
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NativeName { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsEnabled { get; set; }
    public int ResourceCount { get; set; }
    public double CompletionPercentage { get; set; }
}




