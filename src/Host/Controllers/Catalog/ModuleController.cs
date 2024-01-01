using ICISAdminPortal.Application.Catalog.Module;

namespace ICISAdminPortal.Host.Controllers.Catalog;
public class ModuleController : VersionedApiController
{
    [HttpPost]
    [Authorize]
    public Task<Guid> CreateAsync(CreateModuleRequest request)
    {
        return Mediator.Send(request);
    }

}
