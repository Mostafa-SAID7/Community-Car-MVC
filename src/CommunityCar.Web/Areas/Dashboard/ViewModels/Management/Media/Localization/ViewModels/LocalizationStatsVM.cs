namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Management.Media.Localization.ViewModels;

public class LocalizationStatsVM
{
    public int TotalResources { get; set; }
    public int TotalCultures { get; set; }
    public int ActiveCultures { get; set; }
    public Dictionary<string, int> ResourcesByCulture { get; set; } = new();
    public Dictionary<string, double> CompletionByCulture { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}




