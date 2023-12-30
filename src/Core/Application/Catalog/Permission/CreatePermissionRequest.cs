namespace Mukesh.Application.Catalog.Permission;
public record CreatePermissionRequest(CreatePermissionRequestDto model) : IRequest<DefaultIdType>;

public sealed class CreatePermissionHandler : IRequestHandler<CreatePermissionRequest, DefaultIdType>
{
    private readonly IRepositoryWithEvents<Domain.Catalog.Permission> _repository;
    private readonly IRepositoryWithEvents<Page> _repositoryPage;
    private readonly IRepositoryWithEvents<Domain.Catalog.ActionPage> _repositoryAction;
    public CreatePermissionHandler(IRepositoryWithEvents<Domain.Catalog.Permission> repository, IRepositoryWithEvents<Page> repositoryPage, IRepositoryWithEvents<Domain.Catalog.ActionPage> repositoryAction)
    {
        _repository = repository;
        _repositoryPage = repositoryPage;
        _repositoryAction = repositoryAction;
    }

    public async Task<DefaultIdType> Handle(CreatePermissionRequest request, CancellationToken cancellationToken)
    {
        var entity = request.model;
        try
        {

            var action = await _repositoryAction.GetByIdAsync(entity.actionId);
            var page = await _repositoryPage.GetByIdAsync(entity.pageId);

            var permission = new Domain.Catalog.Permission
            {
                NameAr = $"{action.NameAr} {page.NameAr}",
                NameEn = $"{action.NameEn} {page.NameEn}",
                ActionPageId = entity.actionId,
                PageId = entity.pageId,
                IsActive = true
            };

            await _repository.AddAsync(permission, cancellationToken);

            return permission.Id;
        }
        catch (Exception ex)
        {

            throw;
        }
    }
}