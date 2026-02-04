namespace CommunityCar.Web.Models;

public class SidebarConfig
{
    public string Title { get; set; } = "";
    public List<SidebarSection> Sections { get; set; } = new();
    public bool ShowUserProfile { get; set; } = true;
    public bool ShowBackToSite { get; set; } = false;
    public string Theme { get; set; } = "light"; // light, dark, dashboard
}

public class SidebarSection
{
    public string Title { get; set; } = "";
    public List<SidebarItem> Items { get; set; } = new();
}

public class SidebarItem
{
    public string Controller { get; set; } = "";
    public string Action { get; set; } = "Index";
    public string Icon { get; set; } = "circle";
    public string Text { get; set; } = "";
    public string RouteId { get; set; } = "";
    public int Badge { get; set; } = 0;
    public string Url { get; set; } = ""; // For external links
    public bool IsExternal { get; set; } = false;
}