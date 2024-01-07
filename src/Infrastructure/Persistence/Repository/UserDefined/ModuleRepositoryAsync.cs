using ICISAdminPortal.Application.Common.Persistence.UserDefined;
using ICISAdminPortal.Domain.Catalog;
using ICISAdminPortal.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ICISAdminPortal.Infrastructure.Persistence.Repository.UserDefined;
public class ModuleRepositoryAsync : ApplicationDbRepository<Module>, IModuleRepositoryAsync
{
    private readonly DbSet<Module> _dbSetter;

    public ModuleRepositoryAsync(ApplicationDbContext dbContext)
        : base(dbContext)
    {
        _dbSetter = dbContext.Set<Module>();
    }

    public async Task<bool> IsUniqueModuleAsync(string name)
    {
        return await _dbSetter
           .AllAsync(p => p.NameEn != name);
    }
}
