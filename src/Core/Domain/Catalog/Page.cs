namespace ICISAdminPortal.Domain.Catalog;
public class Page : BaseTypeEntity, IAggregateRoot
{
    public DefaultIdType ModuleId { get; set; }
    public virtual Module Module { get; set; }
    public string? Resource { get; set; }
    public ICollection<Permission> Permissions { get; set; }
}
