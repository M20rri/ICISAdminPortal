namespace Mukesh.Domain.Catalog;
public class Permission : BaseTypeEntity, IAggregateRoot
{
    public Guid ActionPageId { get; set; }
    public Guid PageId { get; set; }
    public int Code { get; set; }
    public ActionPage ActionPage { get; set; }
    public Page Page { get; set; }
}
