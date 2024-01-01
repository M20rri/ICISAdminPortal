namespace ICISAdminPortal.Domain.Common.Contracts;
public class BaseTypeEntity : AuditableEntity
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public bool IsActive { get; set; }
}
