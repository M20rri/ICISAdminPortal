namespace Mukesh.Application.Catalog.Pages;
internal class CreatePageRequestValidator : AbstractValidator<CreatePageRequestDto>
{
    public CreatePageRequestValidator()
    {
        RuleFor(x => x.NameEn).NotNull().NotEmpty().WithMessage(x => "Page En Name is required");
        RuleFor(x => x.NameAr).NotNull().NotEmpty().WithMessage(x => "Page Ar Name is required");
        RuleFor(x => x.moduleId).NotNull().NotEmpty().WithMessage(x => "Module is required");
    }
}