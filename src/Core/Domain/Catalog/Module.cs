namespace Mukesh.Domain.Catalog;
public class Module : BaseTypeEntity, IAggregateRoot
{
    public int Code { get; set; }
    public ICollection<Page> Pages { get; set; }
}
