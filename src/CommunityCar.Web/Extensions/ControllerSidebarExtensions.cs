using CommunityCar.Web.Helpers;
using CommunityCar.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommunityCar.Web.Extensions;

public static class ControllerSidebarExtensions
{
    public static void SetMainSidebar(this Controller controller)
    {
        controller.ViewData["SidebarConfig"] = SidebarHelper.GetMainSidebarConfig();
    }

    public static void SetDashboardSidebar(this Controller controller)
    {
        controller.ViewData["SidebarConfig"] = SidebarHelper.GetDashboardSidebarConfig();
    }

    public static void SetProfileSidebar(this Controller controller, string userId, bool isOwnProfile = false)
    {
        controller.ViewData["SidebarConfig"] = SidebarHelper.GetProfileSidebarConfig(userId, isOwnProfile);
    }

    public static void SetChatSidebar(this Controller controller)
    {
        controller.ViewData["SidebarConfig"] = SidebarHelper.GetChatSidebarConfig();
    }

    public static void SetCustomSidebar(this Controller controller, SidebarConfig config)
    {
        controller.ViewData["SidebarConfig"] = config;
    }
}