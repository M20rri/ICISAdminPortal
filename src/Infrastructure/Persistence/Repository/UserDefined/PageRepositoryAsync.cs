using ICISAdminPortal.Application.Common.Persistence.UserDefined;
using ICISAdminPortal.Domain.Catalog;
using ICISAdminPortal.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ICISAdminPortal.Infrastructure.Persistence.Repository.UserDefined;
public class PageRepositoryAsync : ApplicationDbRepository<Page>, IPageRepositoryAsync
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<Page> _dbSetter;
    public PageRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
        _dbSetter = dbContext.Set<Page>();
    }

    public async Task<bool> IsUniquePageAsync(string name, DefaultIdType moduleId)
    {
        return !await _dbSetter
             .AnyAsync(p => p.NameEn == name && p.ModuleId == moduleId);
    }
}
