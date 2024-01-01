using ICISAdminPortal.Application.Catalog.Permission;

namespace ICISAdminPortal.Host.Controllers.Catalog;
public class PermissionController : VersionedApiController
{
    public Task<Guid> CreateAsync(CreatePermissionRequest request)
    {
        return Mediator.Send(request);
    }
}
