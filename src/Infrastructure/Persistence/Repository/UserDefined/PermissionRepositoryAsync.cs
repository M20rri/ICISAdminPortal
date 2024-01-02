using ICISAdminPortal.Application.Common.Persistence.UserDefined;
using ICISAdminPortal.Domain.Catalog;
using ICISAdminPortal.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ICISAdminPortal.Infrastructure.Persistence.Repository.UserDefined;
internal class PermissionRepositoryAsync : ApplicationDbRepository<Permission>, IPermissionRepositoryAsync
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<Permission> _dbSetter;
    public PermissionRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
        _dbSetter = dbContext.Set<Permission>();
    }

    public async Task<bool> IsUniquePermissionAsync(DefaultIdType actionId, DefaultIdType pageId)
    {
        return !await _dbSetter
           .AnyAsync(p => p.PageId == pageId && p.ActionPageId == actionId);
    }
}
