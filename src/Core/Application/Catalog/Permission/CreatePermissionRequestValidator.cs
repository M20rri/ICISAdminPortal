namespace Mukesh.Application.Catalog.Permission;
internal class CreatePermissionRequestValidator : AbstractValidator<CreatePermissionRequestDto>
{
    public CreatePermissionRequestValidator()
    {
        RuleFor(x => x.actionId).NotNull().NotEmpty().WithMessage(x => "Action is required");
        RuleFor(x => x.pageId).NotNull().NotEmpty().WithMessage(x => "page is required");
    }
}