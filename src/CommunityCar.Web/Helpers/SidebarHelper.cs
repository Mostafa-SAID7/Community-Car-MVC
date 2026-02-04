using CommunityCar.Web.Models;

namespace CommunityCar.Web.Helpers;

public static class SidebarHelper
{
    public static SidebarConfig GetMainSidebarConfig()
    {
        return new SidebarConfig
        {
            Title = "",
            ShowUserProfile = true,
            ShowBackToSite = false,
            Theme = "light",
            Sections = new List<SidebarSection>
            {
                new SidebarSection
                {
                    Title = "",
                    Items = new List<SidebarItem>
                    {
                        new SidebarItem { Controller = "Home", Action = "Index", Icon = "home", Text = "Home" },
                        new SidebarItem { Controller = "QA", Action = "Index", Icon = "help-circle", Text = "QA" },
                        new SidebarItem { Controller = "Posts", Action = "Index", Icon = "pen-tool", Text = "Posts" },
                        new SidebarItem { Controller = "Guides", Action = "Index", Icon = "book-open", Text = "Guides" },
                        new SidebarItem { Controller = "Groups", Action = "Index", Icon = "users", Text = "Groups" }
                    }
                },
                new SidebarSection
                {
                    Title = "",
                    Items = new List<SidebarItem>
                    {
                        new SidebarItem { Controller = "News", Action = "Index", Icon = "newspaper", Text = "News" },
                        new SidebarItem { Controller = "Events", Action = "Index", Icon = "calendar", Text = "Events" },
                        new SidebarItem { Controller = "Reviews", Action = "Index", Icon = "star", Text = "Reviews" },
                        new SidebarItem { Controller = "Maps", Action = "Index", Icon = "map", Text = "Maps" }
                    }
                }
            }
        };
    }

    public static SidebarConfig GetDashboardSidebarConfig()
    {
        return new SidebarConfig
        {
            Title = "",
            ShowUserProfile = false,
            ShowBackToSite = true,
            Theme = "dashboard",
            Sections = new List<SidebarSection>
            {
                new SidebarSection
                {
                    Title = "Management",
                    Items = new List<SidebarItem>
                    {
                        new SidebarItem { Controller = "Overview", Action = "Index", Icon = "layout-dashboard", Text = "Overview" },
                        new SidebarItem { Controller = "Analytics", Action = "Index", Icon = "bar-chart-3", Text = "Analytics" },
                        new SidebarItem { Controller = "AIManagement", Action = "Index", Icon = "brain", Text = "AI Control" }
                    }
                },
                new SidebarSection
                {
                    Title = "System",
                    Items = new List<SidebarItem>
                    {
                        new SidebarItem { Controller = "Management", Action = "Index", Icon = "settings-2", Text = "AdminControl" },
                        new SidebarItem { Controller = "Errors", Action = "Index", Icon = "alert-triangle", Text = "ErrorLogs" },
                        new SidebarItem { Controller = "Monitoring", Action = "Index", Icon = "activity", Text = "Monitoring" },
                        new SidebarItem { Controller = "Performance", Action = "Index", Icon = "gauge", Text = "Performance" }
                    }
                }
            }
        };
    }

    public static SidebarConfig GetProfileSidebarConfig(string userId, bool isOwnProfile = false)
    {
        var items = new List<SidebarItem>
        {
            new SidebarItem { Controller = "Profile", Action = "Index", Icon = "user", Text = "Profile", RouteId = userId },
            new SidebarItem { Controller = "Profile", Action = "Gallery", Icon = "image", Text = "Gallery", RouteId = userId },
            new SidebarItem { Controller = "Profile", Action = "Interests", Icon = "heart", Text = "Interests", RouteId = userId }
        };

        if (isOwnProfile)
        {
            items.Add(new SidebarItem { Controller = "ProfileSettings", Action = "Index", Icon = "settings", Text = "Settings" });
        }

        return new SidebarConfig
        {
            Title = "",
            ShowUserProfile = false,
            ShowBackToSite = false,
            Theme = "profile",
            Sections = new List<SidebarSection>
            {
                new SidebarSection
                {
                    Title = "Profile",
                    Items = items
                }
            }
        };
    }

    public static SidebarConfig GetChatSidebarConfig()
    {
        return new SidebarConfig
        {
            Title = "Messages",
            ShowUserProfile = true,
            ShowBackToSite = false,
            Theme = "light",
            Sections = new List<SidebarSection>
            {
                new SidebarSection
                {
                    Title = "",
                    Items = new List<SidebarItem>
                    {
                        new SidebarItem { Controller = "Chats", Action = "Index", Icon = "message-circle", Text = "All Chats" },
                        new SidebarItem { Controller = "Chats", Action = "Friends", Icon = "users", Text = "Friends" },
                        new SidebarItem { Controller = "Chats", Action = "Groups", Icon = "users-2", Text = "Group Chats" }
                    }
                }
            }
        };
    }
}