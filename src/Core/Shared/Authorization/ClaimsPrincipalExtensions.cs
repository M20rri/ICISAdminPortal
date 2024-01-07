using ICISAdminPortal.Shared.Authorization;

namespace System.Security.Claims;
public static class ClaimsPrincipalExtensions
{
    public static string? GetEmail(this ClaimsPrincipal principal)
        => principal.FindFirstValue("email");

    public static string? GetTenant(this ClaimsPrincipal principal)
        => principal.FindFirstValue("tenant");

    public static string? GetFullName(this ClaimsPrincipal principal)
        => principal?.FindFirst("fullName")?.Value;

    public static string? GetUsername(this ClaimsPrincipal principal)
        => principal?.FindFirst("userName")?.Value;

    public static string? GetUserId(this ClaimsPrincipal principal)
       => principal.FindFirstValue("id");

    public static string? GetUserRole(this ClaimsPrincipal principal)
    => principal.FindFirstValue("userRole");

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