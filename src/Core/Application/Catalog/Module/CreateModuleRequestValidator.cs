namespace ICISAdminPortal.Application.Catalog.Module;
internal class CreateModuleRequestValidator : AbstractValidator<CreateModuleRequestDto>
{
    public CreateModuleRequestValidator()
    {
        RuleFor(x => x.NameEn).NotNull().NotEmpty().WithMessage(x => "Module En Name is required");
        RuleFor(x => x.NameAr).NotNull().NotEmpty().WithMessage(x => "Module Ar Name is required");
        RuleFor(x => x.Code).GreaterThan(1).WithMessage(x => "Code is required");
    }
}
