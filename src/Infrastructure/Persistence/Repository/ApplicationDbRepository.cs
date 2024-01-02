using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Mapster;
using ICISAdminPortal.Application.Common.Persistence;
using ICISAdminPortal.Domain.Common.Contracts;
using ICISAdminPortal.Infrastructure.Persistence.Context;

namespace ICISAdminPortal.Infrastructure.Persistence.Repository;
// Inherited from Ardalis.Specification's RepositoryBase<T>
public class ApplicationDbRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
    where T : class, IAggregateRoot
{

    public ApplicationDbContext DbContext { get; set; }

    public ApplicationDbRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
        DbContext = dbContext;
    }

    protected override IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification) =>
        specification.Selector is not null
            ? base.ApplySpecification(specification)
            : ApplySpecification(specification, false)
                .ProjectToType<TResult>();
}