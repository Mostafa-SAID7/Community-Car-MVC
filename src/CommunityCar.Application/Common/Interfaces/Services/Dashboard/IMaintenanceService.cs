namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard;

public interface IMaintenanceService
{
    Task<bool> IsMaintenanceModeEnabledAsync();
    Task SetMaintenanceModeAsync(bool isEnabled);
    Task<string> GetMaintenanceMessageAsync();
    Task SetMaintenanceMessageAsync(string message);
}
