namespace ICISAdminPortal.Domain.Catalog;
public class ActionPage : BaseTypeEntity, IAggregateRoot
{
    public ICollection<Permission> Permissions { get; set; }
}
