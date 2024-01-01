using ICISAdminPortal.Application.Catalog.Permission;
using System.Net;

namespace ICISAdminPortal.Host.Controllers.Catalog;
public class PermissionController : VersionedApiController
{
    public async Task<IActionResult> CreateAsync(CreatePermissionRequest request)
    {
        var response = await Mediator.Send(request);
        return CustomResult("Saved Sucesfully", response, HttpStatusCode.OK);
    }
}
