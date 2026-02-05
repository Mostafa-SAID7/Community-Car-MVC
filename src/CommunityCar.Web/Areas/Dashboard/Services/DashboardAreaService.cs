namespace CommunityCar.Web.Areas.Dashboard.Services;

public interface IDashboardAreaService
{
    string GetAreaStatus();
}

public class DashboardAreaService : IDashboardAreaService
{
    public string GetAreaStatus() => "Dashboard Area is active and organized.";
}




