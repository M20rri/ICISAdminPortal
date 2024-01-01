using Microsoft.AspNetCore.Authorization;
using ICISAdminPortal.Shared.Authorization;

namespace ICISAdminPortal.Infrastructure.Auth.Permissions;
public class MustHavePermissionAttribute : AuthorizeAttribute
{
    public MustHavePermissionAttribute(string action, string resource) =>
        Policy = FSHPermission.NameFor(action, resource);
}