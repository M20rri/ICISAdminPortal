using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ICISAdminPortal.Application.Common.Persistence;
using ICISAdminPortal.Domain.Common.Contracts;
using ICISAdminPortal.Infrastructure.Persistence.Context;
using System.Linq.Expressions;

namespace ICISAdminPortal.Infrastructure.Persistence.Repository;
// Inherited from Ardalis.Specification's RepositoryBase<T>
public class ApplicationDbRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
    where T : class, IAggregateRoot
{

    public ApplicationDbContext _dbContext { get; set; }

    public ApplicationDbRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification) =>
        specification.Selector is not null
            ? base.ApplySpecification(specification)
            : ApplySpecification(specification, false)
                .ProjectToType<TResult>();

    public async Task<T> FindByIdAsync(Expression<Func<T, bool>> criteria, string[] includes = null)
    {
        IQueryable<T> query = _dbContext.Set<T>();

        if (includes != null)
            foreach (var incluse in includes)
                query = query.Include(incluse);

        return await query.FirstOrDefaultAsync(criteria);
    }
}