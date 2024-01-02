namespace ICISAdminPortal.Application.Common.Persistence.UserDefined;
public interface IActionRepositoryAsync : IRepositoryWithEvents<ActionPage>
{
    Task<bool> IsUniqueActionAsync(string name);
}
