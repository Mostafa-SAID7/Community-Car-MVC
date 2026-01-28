using CommunityCar.Infrastructure;
using CommunityCar.Application;
using CommunityCar.AI;
using CommunityCar.Web.Extensions;
using CommunityCar.Infrastructure.Hubs;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

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

// Seed Data - Temporarily disabled for faster startup
// using (var scope = app.Services.CreateScope())
// {
//     var seeder = scope.ServiceProvider.GetRequiredService<CommunityCar.Infrastructure.Persistence.Seeding.DataSeeder>();
//     await seeder.SeedAsync();
// }

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/500");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseMiddleware<CommunityCar.Web.Middleware.ErrorHandlingMiddleware>();
app.UseMiddleware<CommunityCar.Web.Middleware.MaintenanceMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Map SignalR Hubs
app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<NotificationHub>("/hubs/notifications");

app.MapControllerRoute(
    name: "dashboard",
    pattern: "Dashboard/{controller}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();



