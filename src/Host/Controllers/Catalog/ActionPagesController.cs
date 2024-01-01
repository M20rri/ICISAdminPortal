using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ICISAdminPortal.Application.Catalog.ActionPage;
using ICISAdminPortal.Application.Catalog.Module;

namespace ICISAdminPortal.Host.Controllers.Catalog;
public class ActionPagesController : VersionedApiController
{
    public Task<Guid> CreateAsync(CreateActionRequest request)
    {
        return Mediator.Send(request);
    }
}
