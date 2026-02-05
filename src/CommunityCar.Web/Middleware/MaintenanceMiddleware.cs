using CommunityCar.Web.Areas.Dashboard.Interfaces.Services.Maintenance;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CommunityCar.Web.Middleware
{
    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;

        public MaintenanceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IMaintenanceService maintenanceService)
        {
            var isMaintenanceMode = await maintenanceService.IsMaintenanceModeEnabledAsync();
            
            if (isMaintenanceMode)
            {
                var path = context.Request.Path.Value?.ToLower() ?? string.Empty;
                
                // Allow access to:
                // 1. Maintenance page itself
                // 2. Static assets
                // 3. Login/Logout (Admins might need to log in to disable maintenance mode)
                // 4. Admin Dashboard routes (Access restricted by authorization later)
                
                var isAsset = path.Contains("/css/") || path.Contains("/js/") || path.Contains("/lib/") || path.Contains("/assets/") || path.Contains("/favicon");
                var isMaintenancePage = path.Contains("/error/maintenance");
                var isLoginPage = path.Contains("/login") || path.Contains("/identity/account/login");
                var isLogoutPage = path.Contains("/logout") || path.Contains("/identity/account/logout");

                if (!isAsset && !isMaintenancePage)
                {
                    // Allow admins to bypass maintenance mode
                    // Also allow access to login/logout for admins to gain access/session
                    if (context.User.IsInRole("Admin") || isLoginPage || isLogoutPage)
                    {
                        await _next(context);
                        return;
                    }

                    context.Response.Redirect("/Error/Maintenance");
                    return;
                }
            }

            await _next(context);
        }
    }
}




