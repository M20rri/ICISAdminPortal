using Microsoft.AspNetCore.Authorization;
using Mukesh.Shared.Authorization;

namespace Mukesh.Infrastructure.Auth.Permissions;
public class MustHavePermissionAttribute : AuthorizeAttribute
{
    public MustHavePermissionAttribute(string action, string resource) =>
        Policy = FSHPermission.NameFor(action, resource);
}