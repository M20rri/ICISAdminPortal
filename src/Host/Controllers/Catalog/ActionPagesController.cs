using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mukesh.Application.Catalog.ActionPage;
using Mukesh.Application.Catalog.Module;

namespace Mukesh.Host.Controllers.Catalog;
public class ActionPagesController : VersionedApiController
{
    public Task<Guid> CreateAsync(CreateActionRequest request)
    {
        return Mediator.Send(request);
    }
}
