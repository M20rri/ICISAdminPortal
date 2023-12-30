using System.Security.Claims;

namespace Mukesh.Infrastructure.Auth;
public interface ICurrentUserInitializer
{
    void SetCurrentUser(ClaimsPrincipal user);

    void SetCurrentUserId(string userId);
}