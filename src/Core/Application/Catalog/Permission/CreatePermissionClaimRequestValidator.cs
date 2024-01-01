namespace ICISAdminPortal.Application.Catalog.Permission;
public class CreatePermissionClaimRequestValidator : AbstractValidator<CreatePermissionClaimRequestDto>
{
    public CreatePermissionClaimRequestValidator()
    {
        RuleFor(x => x.roleId).NotNull().NotEmpty().WithMessage(x => "Role is required");
        RuleFor(x => x.permissionCode).NotNull().NotEmpty().WithMessage(x => "Permission Code is required");
    }
}
