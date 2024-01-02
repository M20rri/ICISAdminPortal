using ICISAdminPortal.Application.Common.Persistence.UserDefined;

namespace ICISAdminPortal.Application.Catalog.Permission;
internal class CreatePermissionRequestValidator : AbstractValidator<CreatePermissionRequestDto>
{
    private readonly IPermissionRepositoryAsync _repository;
    public CreatePermissionRequestValidator(IPermissionRepositoryAsync repository)
    {
        _repository = repository;

        RuleFor(x => x.actionId).NotNull().NotEmpty().WithMessage(x => "Action is required");
        RuleFor(x => x.pageId).NotNull().NotEmpty().WithMessage(x => "page is required");

        RuleFor(p => p)
      .MustAsync(IsUniqueName).WithMessage("Permission exists before");
    }

    private async Task<bool> IsUniqueName(CreatePermissionRequestDto model, CancellationToken cancellationToken)
    {
        return await _repository.IsUniquePermissionAsync(model.actionId, model.pageId);
    }
}