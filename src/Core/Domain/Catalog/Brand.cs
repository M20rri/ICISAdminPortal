namespace Mukesh.Domain.Catalog;
public class Brand : BaseTypeEntity, IAggregateRoot
{
    public virtual ICollection<Model> Models { get; set; }
}
