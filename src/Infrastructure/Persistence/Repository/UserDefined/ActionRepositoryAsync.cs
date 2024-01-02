using ICISAdminPortal.Application.Common.Persistence.UserDefined;
using ICISAdminPortal.Domain.Catalog;
using ICISAdminPortal.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ICISAdminPortal.Infrastructure.Persistence.Repository.UserDefined;
public class ActionRepositoryAsync : ApplicationDbRepository<ActionPage>, IActionRepositoryAsync
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<ActionPage> _dbSetter;
    public ActionRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
        _dbSetter = dbContext.Set<ActionPage>();
    }

    public async Task<bool> IsUniqueActionAsync(string name)
    {
        return await _dbSetter
          .AllAsync(p => p.NameEn != name);
    }
}
