using Microsoft.AspNetCore.Identity;

namespace ICISAdminPortal.Infrastructure.Identity;
public class ApplicationRoleClaim : IdentityRoleClaim<string>
{
    public string? CreatedBy { get; init; }
    public DateTime CreatedOn { get; init; }
}