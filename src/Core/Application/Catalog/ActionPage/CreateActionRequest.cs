using ICISAdminPortal.Application.Common.Persistence.UserDefined;
using System.Net;

namespace ICISAdminPortal.Application.Catalog.ActionPage;
public record CreateActionRequest(CreateActionRequestDto model) : IRequest<DefaultIdType>;

public sealed class CreateActionHandler : IRequestHandler<CreateActionRequest, DefaultIdType>
{
    private readonly IActionRepositoryAsync _repository;

    public CreateActionHandler(IActionRepositoryAsync repository)
    {
        _repository = repository;
    }
    public async Task<DefaultIdType> Handle(CreateActionRequest request, CancellationToken cancellationToken)
    {
        var entity = request.model;

        CreateActionRequestValidator validationRules = new CreateActionRequestValidator(_repository);
        var result = await validationRules.ValidateAsync(entity);
        if (result.Errors.Any())
        {
            var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new Exceptions.ValidationException(errors, (int)HttpStatusCode.BadRequest);
        }

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