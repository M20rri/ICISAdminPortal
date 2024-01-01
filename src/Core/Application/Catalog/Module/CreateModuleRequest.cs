namespace Mukesh.Application.Catalog.Module;
public record CreateModuleRequest(CreateModuleRequestDto model) : IRequest<DefaultIdType>;

public sealed class CreateModuleHandler : IRequestHandler<CreateModuleRequest, DefaultIdType>
{
    private readonly IRepositoryWithEvents<Domain.Catalog.Module> _repository;
    public CreateModuleHandler(IRepositoryWithEvents<Domain.Catalog.Module> repository)
    {
        _repository = repository;
    }
    public async Task<DefaultIdType> Handle(CreateModuleRequest request, CancellationToken cancellationToken)
    {
        var entity = request.model;

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
