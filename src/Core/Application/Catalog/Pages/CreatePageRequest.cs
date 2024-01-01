﻿using System.Text.RegularExpressions;

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

        CreatePageRequestValidator validationRules = new CreatePageRequestValidator();
        var result = await validationRules.ValidateAsync(entity);
        if (result.Errors.Any())
        {
            var errors = result.Errors;
            throw new ValidationException(errors);
        }

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