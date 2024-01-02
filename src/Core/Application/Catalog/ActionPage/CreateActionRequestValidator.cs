using ICISAdminPortal.Application.Common.Persistence.UserDefined;

namespace ICISAdminPortal.Application.Catalog.ActionPage;
internal class CreateActionRequestValidator : AbstractValidator<CreateActionRequestDto>
{
    private readonly IActionRepositoryAsync _repository;
    public CreateActionRequestValidator(IActionRepositoryAsync repository)
    {
        _repository = repository;

        RuleFor(x => x.NameAr).NotNull().NotEmpty().WithMessage(x => "Action Ar Name is required");
        RuleFor(p => p.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty().WithMessage("Action En Name is required")
           .MustAsync(IsUniqueName).WithMessage("Action exists before");
    }

    private async Task<bool> IsUniqueName(string name, CancellationToken cancellationToken)
    {
        return await _repository.IsUniqueActionAsync(name);
    }
}
