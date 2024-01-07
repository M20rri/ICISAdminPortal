using ICISAdminPortal.Application.Multitenancy;

namespace ICISAdminPortal.Host.Controllers.Multitenancy;
public class TenantsController : VersionNeutralApiController
{
    [HttpGet]
    [MustHavePermission("")]
    [OpenApiOperation("Get a list of all tenants.", "")]
    public Task<List<TenantDto>> GetListAsync()
    {
        return Mediator.Send(new GetAllTenantsRequest());
    }

    [HttpGet("{id}")]
    [MustHavePermission("")]
    [OpenApiOperation("Get tenant details.", "")]
    public Task<TenantDto> GetAsync(string id)
    {
        return Mediator.Send(new GetTenantRequest(id));
    }

    [HttpPost]
    [MustHavePermission("")]
    [OpenApiOperation("Create a new tenant.", "")]
    public Task<string> CreateAsync(CreateTenantRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("{id}/activate")]
    [MustHavePermission("")]
    [OpenApiOperation("Activate a tenant.", "")]
    [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Register))]
    public Task<string> ActivateAsync(string id)
    {
        return Mediator.Send(new ActivateTenantRequest(id));
    }

    [HttpPost("{id}/deactivate")]
    [MustHavePermission("")]
    [OpenApiOperation("Deactivate a tenant.", "")]
    [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Register))]
    public Task<string> DeactivateAsync(string id)
    {
        return Mediator.Send(new DeactivateTenantRequest(id));
    }

    [HttpPost("{id}/upgrade")]
    [MustHavePermission("")]
    [OpenApiOperation("Upgrade a tenant's subscription.", "")]
    [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Register))]
    public async Task<ActionResult<string>> UpgradeSubscriptionAsync(string id, UpgradeSubscriptionRequest request)
    {
        return id != request.TenantId
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }
}