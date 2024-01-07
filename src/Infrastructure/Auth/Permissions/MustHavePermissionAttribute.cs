using Microsoft.AspNetCore.Authorization;
using ICISAdminPortal.Shared.Authorization;

namespace ICISAdminPortal.Infrastructure.Auth.Permissions;
public class MustHavePermissionAttribute : AuthorizeAttribute
{
    public MustHavePermissionAttribute(string claimValue) =>
    Policy = FSHPermission.NameFor(claimValue);
}