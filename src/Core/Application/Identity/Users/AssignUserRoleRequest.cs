namespace ICISAdminPortal.Application.Identity.Users;
public class AssignUserRoleRequest
{
    public List<string> Roles { get; set; }
    public int? DepartmentId { get; set; }
}
