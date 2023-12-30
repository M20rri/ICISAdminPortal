using Mukesh.Application.Catalog.Permission;

namespace Mukesh.Host.Controllers.Catalog;
public class PermissionController : VersionedApiController
{
    public Task<Guid> CreateAsync(CreatePermissionRequest request)
    {
        return Mediator.Send(request);
    }
}
