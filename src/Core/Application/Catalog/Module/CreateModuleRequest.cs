using ICISAdminPortal.Application.Common.Persistence.UserDefined;
using System.Net;

namespace ICISAdminPortal.Application.Catalog.Module;
public record CreateModuleRequest(CreateModuleRequestDto model) : IRequest<DefaultIdType>;

public sealed class CreateModuleHandler : IRequestHandler<CreateModuleRequest, DefaultIdType>
{
    private readonly IModuleRepositoryAsync _repository;
    public CreateModuleHandler(IModuleRepositoryAsync repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(CreateModuleRequest request, CancellationToken cancellationToken)
    {
        var entity = request.model;

        CreateModuleRequestValidator validationRules = new CreateModuleRequestValidator(_repository);
        var result = await validationRules.ValidateAsync(entity);
        if (result.Errors.Any())
        {
            var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new Exceptions.ValidationException(errors, (int)HttpStatusCode.BadRequest);
        }

        var module = new Domain.Catalog.Module
        {
            Code = entity.Code,
            NameAr = entity.NameAr,
            NameEn = entity.NameEn,
            IsActive = true
        };

        await _repository.AddAsync(module, cancellationToken);

        return module.Id;
    }
}
