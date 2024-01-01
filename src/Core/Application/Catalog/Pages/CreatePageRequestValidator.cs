using ICISAdminPortal.Application.Common.Persistence.UserDefined;

namespace ICISAdminPortal.Application.Catalog.Pages;
internal class CreatePageRequestValidator : AbstractValidator<CreatePageRequestDto>
{
    private readonly IPageRepositoryAsync _repository;
    public CreatePageRequestValidator(IPageRepositoryAsync repository)
    {
        _repository = repository;

        RuleFor(x => x.NameEn).NotNull().NotEmpty().WithMessage(x => "Page En Name is required");
        RuleFor(x => x.NameAr).NotNull().NotEmpty().WithMessage(x => "Page Ar Name is required");
        RuleFor(x => x.moduleId).NotNull().NotEmpty().WithMessage(x => "Module is required");

        RuleFor(p => p)
         .MustAsync(IsUniqueName).WithMessage("Page exists before");
    }

    private async Task<bool> IsUniqueName(CreatePageRequestDto model , CancellationToken cancellationToken)
    {
        return await _repository.IsUniquePageAsync(model.NameEn, model.moduleId);
    }
}