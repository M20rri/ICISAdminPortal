namespace ICISAdminPortal.Application.Common.Persistence.UserDefined;
public interface IPermissionRepositoryAsync : IRepositoryWithEvents<Permission>
{
    Task<bool> IsUniquePermissionAsync(DefaultIdType actionId, DefaultIdType pageId);
}
