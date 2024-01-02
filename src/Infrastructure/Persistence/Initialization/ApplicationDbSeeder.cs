using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ICISAdminPortal.Domain.Catalog;
using ICISAdminPortal.Infrastructure.Identity;
using ICISAdminPortal.Infrastructure.Multitenancy;
using ICISAdminPortal.Infrastructure.Persistence.Context;
using ICISAdminPortal.Shared.Authorization;

namespace ICISAdminPortal.Infrastructure.Persistence.Initialization;
internal class ApplicationDbSeeder
{
    private readonly FSHTenantInfo _currentTenant;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly CustomSeederRunner _seederRunner;
    private readonly ILogger<ApplicationDbSeeder> _logger;

    public ApplicationDbSeeder(FSHTenantInfo currentTenant, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, CustomSeederRunner seederRunner, ILogger<ApplicationDbSeeder> logger)
    {
        _currentTenant = currentTenant;
        _roleManager = roleManager;
        _userManager = userManager;
        _seederRunner = seederRunner;
        _logger = logger;
    }

    public async Task SeedDatabaseAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        //await SeedModuleAsync(dbContext);
        await SeedRolesAsync(dbContext);
        await SeedAdminUserAsync();
        await _seederRunner.RunSeedersAsync(cancellationToken);
    }

    private async Task SeedRolesAsync(ApplicationDbContext dbContext)
    {
        foreach (string roleName in FSHRoles.DefaultRoles)
        {
            if (await _roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName)
                is not ApplicationRole role)
            {
                // Create the role
                _logger.LogInformation("Seeding {role} Role for '{tenantId}' Tenant.", roleName, _currentTenant.Id);
                role = new ApplicationRole(roleName, $"{roleName} Role for {_currentTenant.Id} Tenant");
                await _roleManager.CreateAsync(role);
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        var resultList = FSHRoles.DefaultRoles.Skip(1).ToList();
        string adminUserName = "M2ri";
        var user = await _userManager.FindByNameAsync(adminUserName);

        if (string.IsNullOrWhiteSpace(_currentTenant.Id) || string.IsNullOrWhiteSpace(_currentTenant.AdminEmail))
        {
            return;
        }

        if (await _userManager.Users.FirstOrDefaultAsync(u => u.Email == _currentTenant.AdminEmail)
            is not ApplicationUser adminUser)
        {

            if (user is null)
            {
                user = new ApplicationUser
                {
                    FirstName = "Mahmoud",
                    LastName = "ElTorri",
                    Email = "Mahmoud.ElTorri@icisys.net",
                    UserName = "M2ri",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    NormalizedEmail = "Mahmoud.ElTorri@icisys.net".ToUpperInvariant(),
                    NormalizedUserName = "M2ri".ToUpperInvariant(),
                    IsActive = true
                };

                _logger.LogInformation("Seeding Default Admin User for '{tenantId}' Tenant.", _currentTenant.Id);
                var password = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = password.HashPassword(user, "Torri@12345");
                await _userManager.CreateAsync(user);
            }
        }

        // Assign role to user
        await AssignUserToRoleAsync(resultList, user.Id);

    }

    private async Task AssignUserToRoleAsync(IReadOnlyList<string> roles, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        foreach (string roleName in roles)
        {
            try
            {
                // Assign the role to the user
                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Role '{roleName}' assigned to user '{user.UserName}' successfully.");
                }
                else
                {
                    _logger.LogError($"Failed to assign role '{roleName}' to user '{user.UserName}'.");
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}