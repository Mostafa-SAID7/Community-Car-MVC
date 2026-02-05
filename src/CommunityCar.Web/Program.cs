using CommunityCar.Infrastructure;
using CommunityCar.Application;
using CommunityCar.AI;
using CommunityCar.Web.Extensions;
using CommunityCar.Infrastructure.Hubs;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var mvcBuilder = builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// Add hot reload in development
// Note: AddRazorRuntimeCompilation requires Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation package
if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

// Add SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddAIServices(builder.Configuration);
builder.Services.AddAppLocalization(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<CommunityCar.Infrastructure.Persistence.Seeding.DataSeeder>();
    await seeder.SeedAsync();
    
    // Configure background jobs if enabled
    try
    {
        // Background jobs configuration is not available yet
        // var backgroundJobSettings = builder.Configuration.GetSection("BackgroundJobs").Get<CommunityCar.Infrastructure.Configuration.BackgroundJobSettings>();
        // if (backgroundJobSettings?.EnableScheduledJobs == true)
        // {
        //     CommunityCar.Infrastructure.Configuration.BackgroundJobConfiguration.ConfigureRecurringJobs(scope.ServiceProvider);
        // }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Failed to configure background jobs - continuing without them");
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/500");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseMiddleware<CommunityCar.Web.Middleware.ErrorHandlingMiddleware>();
// app.UseMiddleware<CommunityCar.Web.Middleware.MaintenanceMiddleware>(); // Temporarily disabled due to missing SystemSettings table

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Add Hangfire Dashboard if background jobs are enabled
try
{
    var backgroundJobSettings = builder.Configuration.GetSection("BackgroundJobs").Get<CommunityCar.Infrastructure.Configuration.BackgroundJobSettings>();
    if (backgroundJobSettings?.EnableScheduledJobs == true)
    {
        app.UseHangfireDashboard("/hangfire", new Hangfire.DashboardOptions
        {
            Authorization = new[] { new Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter() }
        });
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(ex, "Failed to configure Hangfire dashboard");
}

app.MapStaticAssets();

// Map SignalR Hubs
app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapHub<BroadcastHub>("/hubs/broadcast");

// Map attribute-routed controllers FIRST (QA, Posts, etc. with [Route] attributes)
app.MapControllers();


// Dashboard area route for all Dashboard controllers
app.MapControllerRoute(
    name: "dashboard",
    pattern: "{culture=en-US}/Dashboard/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{culture=en-US}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();



