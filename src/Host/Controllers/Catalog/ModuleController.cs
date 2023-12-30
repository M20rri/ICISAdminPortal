using Mukesh.Application.Catalog.Module;

namespace Mukesh.Host.Controllers.Catalog;
public class ModuleController : VersionedApiController
{
    [HttpPost]
    [MustHavePermission(FSHAction.Create, FSHResource.Brands)]
    [OpenApiOperation("Create a new brand.", "")]
    public Task<Guid> CreateAsync(CreateModuleRequest request)
    {
        return Mediator.Send(request);
    }

}
