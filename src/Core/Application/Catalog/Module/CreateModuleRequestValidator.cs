using ICISAdminPortal.Application.Common.Persistence.UserDefined;

namespace ICISAdminPortal.Application.Catalog.Module;
internal class CreateModuleRequestValidator : AbstractValidator<CreateModuleRequestDto>
{
    private readonly IModuleRepositoryAsync _repository;
    public CreateModuleRequestValidator(IModuleRepositoryAsync repository)
    {
        _repository = repository;

        RuleFor(x => x.NameAr).NotNull().NotEmpty().WithMessage(x => "Module Ar Name is required");
        RuleFor(x => x.Code).GreaterThan(1).WithMessage(x => "Code is required");
        RuleFor(p => p.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty().WithMessage("Module En Name is required")
           .MustAsync(IsUniqueName).WithMessage("Module exists before");
    }

    private async Task<bool> IsUniqueName(string name, CancellationToken cancellationToken)
    {
        return await _repository.IsUniqueModuleAsync(name);
    }
}
