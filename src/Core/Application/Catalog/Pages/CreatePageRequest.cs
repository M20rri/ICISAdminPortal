using Mukesh.Application.Catalog.Module;
using System.Text.RegularExpressions;

namespace Mukesh.Application.Catalog.Pages;

public record CreatePageRequest(CreatePageRequestDto model) : IRequest<DefaultIdType>;

public sealed class CreatePageHandler : IRequestHandler<CreatePageRequest, DefaultIdType>
{
    private readonly IRepositoryWithEvents<Page> _repository;
    public CreatePageHandler(IRepositoryWithEvents<Page> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(CreatePageRequest request, CancellationToken cancellationToken)
    {
        var entity = request.model;

        var module = new Page
        {
            NameAr = entity.NameAr,
            NameEn = entity.NameEn,
            ModuleId = entity.moduleId,
            Resource = Regex.Replace(entity.NameEn, @"\s+", string.Empty),
            IsActive = true
        };

        await _repository.AddAsync(module, cancellationToken);

        return module.Id;
    }
}