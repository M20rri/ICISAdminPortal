﻿using Microsoft.EntityFrameworkCore;
using ICISAdminPortal.Application.Identity.Users;
using ICISAdminPortal.Domain.Identity;
using ICISAdminPortal.Shared.Authorization;
using ICISAdminPortal.Shared.Multitenancy;
using System.Net;
using ICISAdminPortal.Application.Exceptions;

namespace ICISAdminPortal.Infrastructure.Identity;
internal partial class UserService
{
    public async Task<List<UserRoleDto>> GetRolesAsync(string userId, CancellationToken cancellationToken)
    {
        var userRoles = new List<UserRoleDto>();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) throw new Application.Exceptions.ValidationException("User Not Found.", (int)HttpStatusCode.BadRequest);
        var roles = await _roleManager.Roles.AsNoTracking().ToListAsync(cancellationToken);
        if (roles is null) throw new Application.Exceptions.ValidationException("Roles Not Found.", (int)HttpStatusCode.BadRequest);
        foreach (var role in roles)
        {
            userRoles.Add(new UserRoleDto
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Description = role.Description,
                Enabled = await _userManager.IsInRoleAsync(user, role.Name!)
            });
        }

        return userRoles;
    }

    public async Task<string> AssignRolesAsync(string userId, UserRolesRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new ValidationException(_t["User Not Found."], (int)HttpStatusCode.BadRequest);

        // Check if the user is an admin for which the admin role is getting disabled
        if (await _userManager.IsInRoleAsync(user, "Admin")
            && request.UserRoles.Any(a => !a.Enabled && a.RoleName == "Admin"))
        {
            // Get count of users in Admin Role
            int adminCount = (await _userManager.GetUsersInRoleAsync("Admin")).Count;

            // Check if user is not Root Tenant Admin
            // Edge Case : there are chances for other tenants to have users with the same email as that of Root Tenant Admin. Probably can add a check while User Registration
            if (user.Email == MultitenancyConstants.Root.EmailAddress)
            {
                if (_currentTenant.Id == MultitenancyConstants.Root.Id)
                {
                    throw new ValidationException(_t["Cannot Remove Admin Role From Root Tenant Admin."], (int)HttpStatusCode.BadRequest);
                }
            }
            else if (adminCount <= 2)
            {
                throw new ValidationException(_t["Tenant should have at least 2 Admins."], (int)HttpStatusCode.BadRequest);
            }
        }

        foreach (var userRole in request.UserRoles)
        {
            // Check if Role Exists
            if (await _roleManager.FindByNameAsync(userRole.RoleName!) is not null)
            {
                if (userRole.Enabled)
                {
                    if (!await _userManager.IsInRoleAsync(user, userRole.RoleName!))
                    {
                        await _userManager.AddToRoleAsync(user, userRole.RoleName!);
                    }
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, userRole.RoleName!);
                }
            }
        }

        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id, true));

        return _t["User Roles Updated Successfully."];
    }


}