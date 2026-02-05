namespace CommunityCar.Web.Areas.Dashboard.ViewModels.Overview.Users.Statistics;

/// <summary>
/// User demographics view model
/// </summary>
public class UserDemographicsVM
{
    public Dictionary<string, int> AgeGroups { get; set; } = new();
    public Dictionary<string, int> GenderDistribution { get; set; } = new();
    public Dictionary<string, int> LocationDistribution { get; set; } = new();
    public Dictionary<string, int> DeviceTypes { get; set; } = new();
    public Dictionary<string, int> RegistrationSources { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}




