using ICISAdminPortal.Application.Catalog.Pages;
using System.Net;

namespace ICISAdminPortal.Host.Controllers.Catalog;
public class PageController : VersionNeutralApiController
{
    public async Task<IActionResult> CreateAsync(CreatePageRequest request)
    {
        var response = await Mediator.Send(request);
        return CustomResult("Saved Sucesfully", response, HttpStatusCode.OK);
    }
}
