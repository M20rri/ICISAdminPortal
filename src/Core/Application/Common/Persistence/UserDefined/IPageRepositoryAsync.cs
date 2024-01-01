namespace ICISAdminPortal.Application.Common.Persistence.UserDefined;
public interface IPageRepositoryAsync : IRepositoryWithEvents<Page>
{
    Task<bool> IsUniquePageAsync(string name, DefaultIdType moduleIed);
}
