using CommunityCar.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommunityCar.Infrastructure.Persistence.Seeding.Account.Authorization;

public class RoleSeeder
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ILogger<RoleSeeder> _logger;

    public RoleSeeder(RoleManager<IdentityRole<Guid>> roleManager, ILogger<RoleSeeder> logger)
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
                await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName) { Id = Guid.NewGuid() });
                _logger.LogInformation("Created role: {RoleName}", roleName);
            }
        }
    }
}