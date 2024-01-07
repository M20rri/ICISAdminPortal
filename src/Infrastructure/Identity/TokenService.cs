using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ICISAdminPortal.Application.Identity.Tokens;
using ICISAdminPortal.Infrastructure.Auth;
using ICISAdminPortal.Infrastructure.Auth.Jwt;
using ICISAdminPortal.Infrastructure.Multitenancy;
using ICISAdminPortal.Shared.Multitenancy;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using ICISAdminPortal.Application.Exceptions;

namespace ICISAdminPortal.Infrastructure.Identity;
internal class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer _t;
    private readonly SecuritySettings _securitySettings;
    private readonly JwtSettings _jwtSettings;
    private readonly FSHTenantInfo? _currentTenant;

    public TokenService(
        UserManager<ApplicationUser> userManager,
        IOptions<JwtSettings> jwtSettings,
        IStringLocalizer<TokenService> localizer,
        FSHTenantInfo? currentTenant,
        IOptions<SecuritySettings> securitySettings)
    {
        _userManager = userManager;
        _t = localizer;
        _jwtSettings = jwtSettings.Value;
        _currentTenant = currentTenant;
        _securitySettings = securitySettings.Value;
    }

    public async Task<TokenResponse> GetTokenAsync(TokenRequest request, string ipAddress, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_currentTenant?.Id)
            || await _userManager.FindByEmailAsync(request.Email.Trim().Normalize()) is not { } user
            || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new ValidationException("Authentication Failed.", (int)HttpStatusCode.BadRequest);
        }

        if (!user.IsActive)
        {
            throw new ValidationException("User Not Active. Please contact the administrator.", (int)HttpStatusCode.BadRequest);
        }

        if (_securitySettings.RequireConfirmedAccount && !user.EmailConfirmed)
        {
            throw new ValidationException("E-Mail not confirmed.", (int)HttpStatusCode.BadRequest);
        }

        if (_currentTenant.Id != MultitenancyConstants.Root.Id)
        {
            if (!_currentTenant.IsActive)
            {
                throw new ValidationException("Tenant is not Active. Please contact the Application Administrator.", (int)HttpStatusCode.BadRequest);
            }

            if (DateTime.UtcNow > _currentTenant.ValidUpto)
            {
                throw new ValidationException("Tenant Validity Has Expired. Please contact the Application Administrator.", (int)HttpStatusCode.BadRequest);
            }
        }

        return await GenerateTokensAndUpdateUser(user, request.Role);
    }

    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string role)
    {
        var userPrincipal = GetPrincipalFromExpiredToken(request.Token);
        string? userEmail = userPrincipal.GetEmail();
        var user = await _userManager.FindByEmailAsync(userEmail!);
        if (user is null)
        {
            throw new ValidationException("Authentication Failed.", (int)HttpStatusCode.BadRequest);
        }

        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new ValidationException("Invalid Refresh Token.", (int)HttpStatusCode.BadRequest);
        }

        return await GenerateTokensAndUpdateUser(user, role);
    }

    private async Task<TokenResponse> GenerateTokensAndUpdateUser(ApplicationUser user, string role)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var userClaims = await _userManager.GetClaimsAsync(user);

        var roleClaims = userClaims.Where(c => c.Type == ClaimTypes.Role);


        string token = GenerateJwt(user, role);

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

        await _userManager.UpdateAsync(user);

        return new TokenResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime);
    }

    private string GenerateJwt(ApplicationUser user, string role) =>
        GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user, role));

    private IEnumerable<Claim> GetClaims(ApplicationUser user, string role)
    {
        return new List<Claim>
        {
            new("Id", user.Id),
            new("Email", user.Email!),
            new("Fullname", $"{user.FirstName} {user.LastName}"),
            new("Username", user.UserName!),
            new("TenantId", _currentTenant!.Id),
            new("Role", role)
        };
    }

    private static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
           claims: claims,
           expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
           signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new ValidationException("Invalid Token", (int)HttpStatusCode.BadRequest);
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
}