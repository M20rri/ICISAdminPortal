using ICISAdminPortal.Shared.Authorization;
using System.Net;

namespace ICISAdminPortal.Infrastructure.Identity;
internal partial class UserService
{
    public async Task<List<string>> GetPermissionsAsync(string userId, string role, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new Application.Exceptions.ValidationException("Authentication Failed.", (int)HttpStatusCode.BadRequest);

        var currentRole = await _roleManager.FindByNameAsync(role!);
        var permissions = new List<string>();
        var roleClaims = await _roleManager.GetClaimsAsync(currentRole!);
        var claimValues = roleClaims.Where(c => c.Type == FSHClaims.Permission)?.Select(a => a.Value).Distinct().ToList();
        permissions.AddRange(claimValues!);
        return permissions.Distinct().ToList();
    }

    public async Task<bool> HasPermissionAsync(string userId , string role , string permission, CancellationToken cancellationToken)
    {
        var permissions = await GetPermissionsAsync(userId, role, cancellationToken);
        string claimValue = permission.Split(".")[1];
        return permissions?.Contains(claimValue) ?? false;
    }

    public Task InvalidatePermissionCacheAsync(string userId, CancellationToken cancellationToken) =>
        _cache.RemoveAsync(_cacheKeys.GetCacheKey(FSHClaims.Permission, userId), cancellationToken);
}