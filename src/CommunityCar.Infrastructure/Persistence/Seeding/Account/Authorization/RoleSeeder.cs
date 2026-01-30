using CommunityCar.Domain.Constants;
using CommunityCar.Domain.Entities.Account.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Account.Authorization;

public class RoleSeeder
{
    private readonly RoleManager<Role> _roleManager;
    private readonly ILogger<RoleSeeder> _logger;

    public RoleSeeder(RoleManager<Role> roleManager, ILogger<RoleSeeder> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Seeding roles...");

        var roles = new[]
        {
            Roles.SuperAdmin, Roles.ContentAdmin, Roles.DesignAdmin, Roles.DatabaseAdmin, Roles.Admin,
            Roles.User, Roles.Expert, Roles.Reviewer, Roles.Author, Roles.Master
        };

        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new Role(roleName, $"{roleName} role", "System", true);
                await _roleManager.CreateAsync(role);
                _logger.LogInformation("Created role: {RoleName}", roleName);
            }
        }
    }
}