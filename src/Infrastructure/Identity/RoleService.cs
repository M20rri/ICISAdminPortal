using Finbuckle.MultiTenant;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using ICISAdminPortal.Application.Catalog.Permission;
using ICISAdminPortal.Application.Common.Events;
using ICISAdminPortal.Application.Common.Exceptions;
using ICISAdminPortal.Application.Common.Interfaces;
using ICISAdminPortal.Application.Identity.Roles;
using ICISAdminPortal.Domain.Identity;
using ICISAdminPortal.Infrastructure.Persistence.Context;
using ICISAdminPortal.Shared.Authorization;
using ICISAdminPortal.Shared.Multitenancy;
using FluentValidation;
using System.Net;
using System;

namespace ICISAdminPortal.Infrastructure.Identity;
internal class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;
    private readonly ITenantInfo _currentTenant;
    private readonly IEventPublisher _events;

    public RoleService(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext db,
        IStringLocalizer<RoleService> localizer,
        ICurrentUser currentUser,
        ITenantInfo currentTenant,
        IEventPublisher events)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _db = db;
        _t = localizer;
        _currentUser = currentUser;
        _currentTenant = currentTenant;
        _events = events;
    }

    public async Task<List<RoleDto>> GetListAsync(CancellationToken cancellationToken) =>
        (await _roleManager.Roles.ToListAsync(cancellationToken))
            .Adapt<List<RoleDto>>();

    public async Task<int> GetCountAsync(CancellationToken cancellationToken) =>
        await _roleManager.Roles.CountAsync(cancellationToken);

    public async Task<bool> ExistsAsync(string roleName, string? excludeId) =>
        await _roleManager.FindByNameAsync(roleName)
            is ApplicationRole existingRole
            && existingRole.Id != excludeId;

    public async Task<RoleDto> GetByIdAsync(string id) =>
        await _db.Roles.SingleOrDefaultAsync(x => x.Id == id) is { } role
            ? role.Adapt<RoleDto>()
            : throw new Application.Exceptions.ValidationException(_t["Role Not Found"], (int)HttpStatusCode.BadRequest);

    public async Task<RoleDto> GetByIdWithPermissionsAsync(string roleId, CancellationToken cancellationToken)
    {
        var role = await GetByIdAsync(roleId);

        role.Permissions = await _db.RoleClaims
            .Where(c => c.RoleId == roleId && c.ClaimType == FSHClaims.Permission)
            .Select(c => c.ClaimValue!)
            .ToListAsync(cancellationToken);

        return role;
    }

    public async Task<string> CreateOrUpdateAsync(CreateOrUpdateRoleRequest request)
    {
        if (string.IsNullOrEmpty(request.Id))
        {
            // Create a new role.
            var role = new ApplicationRole(request.Name, request.Description);
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                throw new Application.Exceptions.ValidationException( result.GetErrors(_t), (int)HttpStatusCode.BadRequest);
            }

            await _events.PublishAsync(new ApplicationRoleCreatedEvent(role.Id, role.Name!));

            return string.Format(_t["Role {0} Created."], request.Name);
        }
        else
        {
            // Update an existing role.
            var role = await _roleManager.FindByIdAsync(request.Id);

            _ = role ?? throw new Application.Exceptions.ValidationException(_t["Role Not Found"], (int)HttpStatusCode.BadRequest);

            if (FSHRoles.IsDefault(role.Name!))
            {
                throw new Application.Exceptions.ValidationException(string.Format(_t["Not allowed to modify {0} Role."], role.Name), (int)HttpStatusCode.BadRequest);
            }

            role.Name = request.Name;
            role.NormalizedName = request.Name.ToUpperInvariant();
            role.Description = request.Description;
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                throw new Application.Exceptions.ValidationException(result.GetErrors(_t), (int)HttpStatusCode.BadRequest);
            }

            await _events.PublishAsync(new ApplicationRoleUpdatedEvent(role.Id, role.Name));

            return string.Format(_t["Role {0} Updated."], role.Name);
        }
    }

    public async Task<string> UpdatePermissionsAsync(UpdateRolePermissionsRequest request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.RoleId);
        _ = role ?? throw new Application.Exceptions.ValidationException(_t["Role Not Found"], (int)HttpStatusCode.BadRequest);
        if (role.Name == FSHRoles.Admin)
        {
            throw new Application.Exceptions.ValidationException(_t["Not allowed to modify Permissions for this Role."], (int)HttpStatusCode.BadRequest);
        }

        if (_currentTenant.Id != MultitenancyConstants.Root.Id)
        {
            // Remove Root Permissions if the Role is not created for Root Tenant.
            request.Permissions.RemoveAll(u => u.StartsWith("Permissions.Root."));
        }

        var currentClaims = await _roleManager.GetClaimsAsync(role);

        // Remove permissions that were previously selected
        foreach (var claim in currentClaims.Where(c => !request.Permissions.Any(p => p == c.Value)))
        {
            var removeResult = await _roleManager.RemoveClaimAsync(role, claim);
            if (!removeResult.Succeeded)
            {
                throw new Application.Exceptions.ValidationException(removeResult.GetErrors(_t), (int)HttpStatusCode.BadRequest);
            }
        }

        // Add all permissions that were not previously selected
        foreach (string permission in request.Permissions.Where(c => !currentClaims.Any(p => p.Value == c)))
        {
            if (!string.IsNullOrEmpty(permission))
            {
                _db.RoleClaims.Add(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = FSHClaims.Permission,
                    ClaimValue = permission,
                    CreatedBy = _currentUser.GetUserId().ToString()
                });
                await _db.SaveChangesAsync(cancellationToken);
            }
        }

        await _events.PublishAsync(new ApplicationRoleUpdatedEvent(role.Id, role.Name!, true));

        return _t["Permissions Updated."];
    }

    public async Task<string> DeleteAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        _ = role ?? throw new Application.Exceptions.ValidationException(_t["Role Not Found"], (int)HttpStatusCode.BadRequest);

        if (FSHRoles.IsDefault(role.Name!))
        {
            throw new Application.Exceptions.ValidationException(string.Format(_t["Not allowed to delete {0} Role."], role.Name), (int)HttpStatusCode.BadRequest);
        }

        if ((await _userManager.GetUsersInRoleAsync(role.Name!)).Count > 0)
        {
            throw new Application.Exceptions.ValidationException(string.Format(_t["Not allowed to delete {0} Role as it is being used."], role.Name), (int)HttpStatusCode.BadRequest);
        }

        await _roleManager.DeleteAsync(role);

        await _events.PublishAsync(new ApplicationRoleDeletedEvent(role.Id, role.Name!));

        return string.Format(_t["Role {0} Deleted."], role.Name);
    }

    public async Task<int> CreateClaimsAsync(CreatePermissionClaimRequestDto request, CancellationToken cancellationToken)
    {
        CreatePermissionClaimRequestValidator validationRules = new CreatePermissionClaimRequestValidator();
        var result = await validationRules.ValidateAsync(request);
        if (result.Errors.Any())
        {
            var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new Application.Exceptions.ValidationException(errors, (int)HttpStatusCode.BadRequest);
        }

        var role = await _roleManager.FindByIdAsync(request.roleId);
        ApplicationRoleClaim claim = default!;
        if (role != null)
        {
            claim = new ApplicationRoleClaim
            {
                RoleId = role.Id,
                ClaimType = FSHClaims.Permission,
                ClaimValue = request.permissionCode,
                CreatedBy = _currentUser.GetUserId().ToString(),
                CreatedOn = DateTime.UtcNow
            };

            _db.RoleClaims.Add(claim);
            await _db.SaveChangesAsync(cancellationToken);

        }

        return claim.Id;
    }
}