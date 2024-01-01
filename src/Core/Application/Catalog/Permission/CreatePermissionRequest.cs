namespace ICISAdminPortal.Application.Catalog.Permission;

public record CreatePermissionRequest(CreatePermissionRequestDto model) : IRequest<DefaultIdType>;

public sealed class CreatePermissionHandler : IRequestHandler<CreatePermissionRequest, DefaultIdType>
{
    private readonly IRepositoryWithEvents<Domain.Catalog.Permission> _repository;
    private readonly IReadRepository<Page> _repositoryPage;
    private readonly IRepositoryWithEvents<Domain.Catalog.ActionPage> _repositoryAction;
    public CreatePermissionHandler(IRepositoryWithEvents<Domain.Catalog.Permission> repository, IReadRepository<Page> repositoryPage, IRepositoryWithEvents<Domain.Catalog.ActionPage> repositoryAction)
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
            CreatePermissionRequestValidator validationRules = new CreatePermissionRequestValidator();
            var result = await validationRules.ValidateAsync(entity);
            if (result.Errors.Any())
            {
                var errors = result.Errors;
                throw new ValidationException(errors);
            }


            var action = await _repositoryAction.GetByIdAsync(entity.actionId);

            var page = await _repositoryPage.FindByIdAsync(a => a.Id == entity.pageId, new[] { "Module" });
            int codeModule = page.Module.Code;

            int code = codeModule + 1;
            var permissions = await _repository.ListAsync();
            if (permissions.Any())
            {
                var lastRow = permissions.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                code = lastRow.Code + 1;
            }


            var permission = new Domain.Catalog.Permission
            {
                NameAr = $"{action.NameAr} {page.NameAr}",
                NameEn = $"{action.NameEn} {page.NameEn}",
                Code = code,
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