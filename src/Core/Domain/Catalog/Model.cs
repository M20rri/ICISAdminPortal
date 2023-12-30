namespace Mukesh.Domain.Catalog;
public class Model : BaseTypeEntity, IAggregateRoot
{
    public Guid BrandId { get; set; }
    public virtual Brand Brand { get; set; }
}
