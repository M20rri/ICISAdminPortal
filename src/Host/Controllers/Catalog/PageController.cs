using ICISAdminPortal.Application.Catalog.Pages;

namespace ICISAdminPortal.Host.Controllers.Catalog;
public class PageController : VersionNeutralApiController
{
    public Task<Guid> CreateAsync(CreatePageRequest request)
    {
        return Mediator.Send(request);
    }
}
