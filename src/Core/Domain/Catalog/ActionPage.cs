namespace Mukesh.Domain.Catalog;
public class ActionPage : BaseTypeEntity, IAggregateRoot
{
    public virtual ICollection<Permission> Permissions { get; set; }
}
