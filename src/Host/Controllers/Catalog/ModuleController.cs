using Mukesh.Application.Catalog.Module;

namespace Mukesh.Host.Controllers.Catalog;
public class ModuleController : VersionedApiController
{
    [HttpPost]
    [Authorize]
    public Task<Guid> CreateAsync(CreateModuleRequest request)
    {
        return Mediator.Send(request);
    }

}
