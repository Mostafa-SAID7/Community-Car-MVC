using CommunityCar.Application.Common.Interfaces.Data;
using CommunityCar.Application.Common.Interfaces.Services.Dashboard;
using CommunityCar.Domain.Entities.Dashboard.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace CommunityCar.Application.Services.Dashboard;

public class MaintenanceService : IMaintenanceService
{
    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private const string CacheKey = "MaintenanceMode_State";
    private const string SettingKey = "MaintenanceMode.IsEnabled";
    private const string MessageKey = "MaintenanceMode.Message";

    public MaintenanceService(IApplicationDbContext context, IMemoryCache cache, IConfiguration configuration)
    {
        _context = context;
        _cache = cache;
        _configuration = configuration;
    }

    public async Task<bool> IsMaintenanceModeEnabledAsync()
    {
        if (_cache.TryGetValue(CacheKey, out bool isEnabled))
        {
            return isEnabled;
        }

        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == SettingKey);

        if (setting == null)
        {
            // Fallback to configuration
            isEnabled = _configuration.GetValue<bool>("MaintenanceMode:IsEnabled");
        }
        else
        {
            if (bool.TryParse(setting.Value, out var result))
            {
                isEnabled = result;
            }
            else
            {
                isEnabled = false;
            }
        }

        // Cache for 1 minute to avoid hammering the DB in middleware
        _cache.Set(CacheKey, isEnabled, TimeSpan.FromMinutes(1));
        return isEnabled;
    }

    public async Task SetMaintenanceModeAsync(bool isEnabled)
    {
        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == SettingKey);

        if (setting == null)
        {
            setting = new SystemSetting
            {
                Key = SettingKey,
                Value = isEnabled.ToString(),
                DataType = "bool",
                Category = "System",
                Description = "Whether the application is in maintenance mode"
            };
            _context.SystemSettings.Add(setting);
        }
        else
        {
            setting.UpdateValue(isEnabled.ToString());
        }

        await _context.SaveChangesAsync();
        _cache.Set(CacheKey, isEnabled, TimeSpan.FromMinutes(5));
    }

    public async Task<string> GetMaintenanceMessageAsync()
    {
        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == MessageKey);

        return setting?.Value ?? "The system is currently undergoing scheduled maintenance. We'll be back online shortly.";
    }

    public async Task SetMaintenanceMessageAsync(string message)
    {
        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == MessageKey);

        if (setting == null)
        {
            setting = new SystemSetting
            {
                Key = MessageKey,
                Value = message,
                DataType = "string",
                Category = "System",
                Description = "Message displayed to users during maintenance mode"
            };
            _context.SystemSettings.Add(setting);
        }
        else
        {
            setting.UpdateValue(message);
        }

        await _context.SaveChangesAsync();
    }
}