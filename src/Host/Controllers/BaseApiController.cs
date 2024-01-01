using CoreApiResponse;
using MediatR;

namespace ICISAdminPortal.Host.Controllers;
[ApiController]
public class BaseApiController : BaseController
{
    private ISender _mediator = null!;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}