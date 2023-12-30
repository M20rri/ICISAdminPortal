namespace Mukesh.Application.Catalog.ActionPage;
public record CreateActionRequest(CreateActionRequestDto model) : IRequest<DefaultIdType>;

public sealed class CreateActionHandler : IRequestHandler<CreateActionRequest, DefaultIdType>
{
    private readonly IRepositoryWithEvents<Domain.Catalog.ActionPage> _repository;

    public CreateActionHandler(IRepositoryWithEvents<Domain.Catalog.ActionPage> repository)
    {
        _repository = repository;
    }
    public async Task<DefaultIdType> Handle(CreateActionRequest request, CancellationToken cancellationToken)
    {
        var entity = request.model;

        var actionPage = new Domain.Catalog.ActionPage
        {
            NameAr = entity.NameAr,
            NameEn = entity.NameEn,
            IsActive = true
        };

        await _repository.AddAsync(actionPage, cancellationToken);

        return actionPage.Id;
    }
}