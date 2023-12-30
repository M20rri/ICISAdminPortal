namespace Mukesh.Domain.Catalog;
public class Permission : BaseTypeEntity, IAggregateRoot
{
    public Guid ActionPageId { get; set; }
    public Guid PageId { get; set; }

    public virtual ActionPage ActionPage { get; set; }
    public virtual Page Page { get; set; }
}
