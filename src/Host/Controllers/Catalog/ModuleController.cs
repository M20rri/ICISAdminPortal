using ICISAdminPortal.Application.Catalog.Module;
using System.Net;

namespace ICISAdminPortal.Host.Controllers.Catalog;
public class ModuleController : VersionedApiController
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateAsync(CreateModuleRequest request)
    {
        var response = await Mediator.Send(request);
        return CustomResult("Saved Sucesfully", response, HttpStatusCode.OK);
    }

}
