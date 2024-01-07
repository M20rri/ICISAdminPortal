using ICISAdminPortal.Application.Catalog.Permission;
using ICISAdminPortal.Application.Identity.Roles;
using ICISAdminPortal.Application.Identity.Users;
using System.Net;

namespace ICISAdminPortal.Host.Controllers.Identity;
public class RolesController : VersionNeutralApiController
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService) => _roleService = roleService;

    [HttpGet]
    [MustHavePermission("")]
    [OpenApiOperation("Get a list of all roles.", "")]
    public Task<List<RoleDto>> GetListAsync(CancellationToken cancellationToken)
    {
        return _roleService.GetListAsync(cancellationToken);
    }

    [HttpGet("{id}")]
    [MustHavePermission("")]
    [OpenApiOperation("Get role details.", "")]
    public Task<RoleDto> GetByIdAsync(string id)
    {
        return _roleService.GetByIdAsync(id);
    }

    [HttpGet("{id}/permissions")]
    [MustHavePermission("")]
    [OpenApiOperation("Get role details with its permissions.", "")]
    public Task<RoleDto> GetByIdWithPermissionsAsync(string id, CancellationToken cancellationToken)
    {
        return _roleService.GetByIdWithPermissionsAsync(id, cancellationToken);
    }

    [HttpPut("{id}/permissions")]
    [MustHavePermission("")]
    [OpenApiOperation("Update a role's permissions.", "")]
    public async Task<ActionResult<string>> UpdatePermissionsAsync(string id, UpdateRolePermissionsRequest request, CancellationToken cancellationToken)
    {
        if (id != request.RoleId)
        {
            return BadRequest();
        }

        return Ok(await _roleService.UpdatePermissionsAsync(request, cancellationToken));
    }

    [HttpPost]
    [MustHavePermission("")]
    [OpenApiOperation("Create or update a role.", "")]
    public Task<string> RegisterRoleAsync(CreateOrUpdateRoleRequest request)
    {
        return _roleService.CreateOrUpdateAsync(request);
    }

    [HttpDelete("{id}")]
    [MustHavePermission("")]
    [OpenApiOperation("Delete a role.", "")]
    public Task<string> DeleteAsync(string id)
    {
        return _roleService.DeleteAsync(id);
    }

    [HttpPost]
    [Route("CreateRoleClaimAsync")]
    [OpenApiOperation("Create claims for role.", "")]
    public async Task<IActionResult> CreateRoleClaimAsync(CreatePermissionClaimRequestDto request, CancellationToken cancellationToken)
    {
        int response = await _roleService.CreateClaimsAsync(request, cancellationToken);
        return CustomResult("Saved Sucesfully", response, HttpStatusCode.OK);
    }

    [HttpPost("{id}/assign-role")]
    [OpenApiOperation("Update a user's assigned roles.", "")]
    public async Task<IActionResult> AssignRolesAsync(string id, AssignUserRoleRequest request, CancellationToken cancellationToken)
    {
        await _roleService.AssignUserRolesAsync(id, request, cancellationToken);
        return CustomResult("Saved Sucesfully", HttpStatusCode.OK);
    }
}