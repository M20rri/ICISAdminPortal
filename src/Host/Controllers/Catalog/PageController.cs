using Mukesh.Application.Catalog.Pages;

namespace Mukesh.Host.Controllers.Catalog;
public class PageController : VersionNeutralApiController
{
    public Task<Guid> CreateAsync(CreatePageRequest request)
    {
        return Mediator.Send(request);
    }
}
