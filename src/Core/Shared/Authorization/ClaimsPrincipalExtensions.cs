using ICISAdminPortal.Shared.Authorization;

namespace System.Security.Claims;
public static class ClaimsPrincipalExtensions
{
    public static string? GetEmail(this ClaimsPrincipal principal)
        => principal.FindFirstValue("Email");

    public static string? GetTenant(this ClaimsPrincipal principal)
        => principal.FindFirstValue("TenantId");

    public static string? GetFullName(this ClaimsPrincipal principal)
        => principal?.FindFirst("Fullname")?.Value;

    public static string? GetUsername(this ClaimsPrincipal principal)
        => principal?.FindFirst("Username")?.Value;

    public static string? GetUserId(this ClaimsPrincipal principal)
       => principal.FindFirstValue("Id");

    public static List<string>? GetUserClaimValues(this ClaimsPrincipal principal)
       => principal.FindAll("ClaimValue").Select(a => a.Value).ToList();

    public static DateTimeOffset GetExpiration(this ClaimsPrincipal principal) =>
        DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(
            principal.FindFirstValue(FSHClaims.Expiration)));

    private static string? FindFirstValue(this ClaimsPrincipal principal, string claimType) =>
        principal is null
            ? throw new ArgumentNullException(nameof(principal))
            : principal.FindFirst(claimType)?.Value;
}