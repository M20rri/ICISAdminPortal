using Microsoft.AspNetCore.Identity;

namespace ICISAdminPortal.Infrastructure.Identity;
public class ApplicationUserRole : IdentityUserRole<string>
{
    public int? DepartmentId { get; set; }
    public string CheckMigrate { get; set; }
}
