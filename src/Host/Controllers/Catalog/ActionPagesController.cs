using ICISAdminPortal.Application.Catalog.ActionPage;
using System.Net;

namespace ICISAdminPortal.Host.Controllers.Catalog;
public class ActionPagesController : VersionedApiController
{
    public async Task<IActionResult> CreateAsync(CreateActionRequest request)
    {
        var response = await Mediator.Send(request);
        return CustomResult("Saved Sucesfully", response, HttpStatusCode.OK);
    }
}
