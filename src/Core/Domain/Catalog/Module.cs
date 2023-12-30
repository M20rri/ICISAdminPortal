namespace Mukesh.Domain.Catalog;
public class Module : BaseTypeEntity, IAggregateRoot
{
    public virtual ICollection<Page> Pages { get; set; }
}
