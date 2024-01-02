using ICISAdminPortal.Application.Catalog.Permission;
using ICISAdminPortal.Application.Identity.Users;

namespace ICISAdminPortal.Application.Identity.Roles;

public interface IRoleService : ITransientService
{
    Task<List<RoleDto>> GetListAsync(CancellationToken cancellationToken);

    Task<int> GetCountAsync(CancellationToken cancellationToken);

    Task<bool> ExistsAsync(string roleName, string? excludeId);

    Task<RoleDto> GetByIdAsync(string id);

    Task<RoleDto> GetByIdWithPermissionsAsync(string roleId, CancellationToken cancellationToken);

    Task<string> CreateOrUpdateAsync(CreateOrUpdateRoleRequest request);

    Task<string> UpdatePermissionsAsync(UpdateRolePermissionsRequest request, CancellationToken cancellationToken);

    Task<string> DeleteAsync(string id);

    Task<int> CreateClaimsAsync(CreatePermissionClaimRequestDto request, CancellationToken cancellationToken);

    Task AssignUserRolesAsync(string userId, AssignUserRoleRequest request, CancellationToken cancellationToken);
}