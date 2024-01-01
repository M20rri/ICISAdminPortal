namespace ICISAdminPortal.Domain.Catalog;
public class Brand : BaseTypeEntity, IAggregateRoot
{
    public ICollection<Model> Models { get; set; }
}
