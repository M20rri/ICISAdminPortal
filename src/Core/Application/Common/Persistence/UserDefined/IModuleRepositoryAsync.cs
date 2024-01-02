namespace ICISAdminPortal.Application.Common.Persistence.UserDefined;
public interface IModuleRepositoryAsync : IRepositoryWithEvents<Module>
{
    Task<bool> IsUniqueModuleAsync(string name);
}
