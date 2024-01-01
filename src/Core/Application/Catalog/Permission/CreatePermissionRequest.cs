using ICISAdminPortal.Application.Common.Persistence.UserDefined;
using System.Net;

namespace ICISAdminPortal.Application.Catalog.Permission;

public record CreatePermissionRequest(CreatePermissionRequestDto model) : IRequest<DefaultIdType>;

public sealed class CreatePermissionHandler : IRequestHandler<CreatePermissionRequest, DefaultIdType>
{
    private readonly IPermissionRepositoryAsync _repository;
    private readonly IPageRepositoryAsync _repositoryPage;
    private readonly IActionRepositoryAsync _repositoryAction;
    public CreatePermissionHandler(IPermissionRepositoryAsync repository, IPageRepositoryAsync repositoryPage, IActionRepositoryAsync repositoryAction)
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
            CreatePermissionRequestValidator validationRules = new CreatePermissionRequestValidator(_repository);
            var result = await validationRules.ValidateAsync(entity);
            if (result.Errors.Any())
            {
                var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
                throw new Exceptions.ValidationException(errors, (int)HttpStatusCode.BadRequest);
            }


            var action = await _repositoryAction.GetByIdAsync(entity.actionId) ??
                throw new Exceptions.ValidationException("Module Not Exist", (int)HttpStatusCode.BadRequest);

            var page = await _repositoryPage.FindWithIncludesAsync(a => a.Id == entity.pageId, new[] { "Module" }) ??
                throw new Exceptions.ValidationException("Page Not Exist", (int)HttpStatusCode.BadRequest);

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