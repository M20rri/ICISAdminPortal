namespace ICISAdminPortal.Application.Catalog.ActionPage;
internal class CreateActionRequestValidator : AbstractValidator<CreateActionRequestDto>
{
    public CreateActionRequestValidator()
    {
        RuleFor(x => x.NameEn).NotNull().NotEmpty().WithMessage(x => "Action En Name is required");
        RuleFor(x => x.NameAr).NotNull().NotEmpty().WithMessage(x => "Action Ar Name is required");
    }
}
