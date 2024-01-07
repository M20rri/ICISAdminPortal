namespace ICISAdminPortal.Shared.Authorization;
public record FSHPermission(string Description, string Action, string Resource, bool IsBasic = false, bool IsRoot = false)
{
    public static string NameFor(string claimValue) => $"Permissions.{claimValue}";
}
