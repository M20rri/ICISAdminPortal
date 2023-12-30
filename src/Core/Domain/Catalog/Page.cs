namespace Mukesh.Domain.Catalog;
public class Page : BaseTypeEntity, IAggregateRoot
{
    public Guid ModuleId { get; set; }
    public virtual Module Module { get; set; }
    public string? Resource { get; set; }
    public virtual ICollection<Permission> Permissions { get; set; }
}
