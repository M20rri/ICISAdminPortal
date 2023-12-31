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
using StackExchange.Redis;
using ICISAdminPortal.Shared.Authorization;

namespace ICISAdminPortal.Infrastructure.Identity;
internal class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IStringLocalizer _t;
    private readonly SecuritySettings _securitySettings;
    private readonly JwtSettings _jwtSettings;
    private readonly FSHTenantInfo? _currentTenant;

    public TokenService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<JwtSettings> jwtSettings,
        IStringLocalizer<TokenService> localizer,
        FSHTenantInfo? currentTenant,
        IOptions<SecuritySettings> securitySettings)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _t = localizer;
        _jwtSettings = jwtSettings.Value;
        _currentTenant = currentTenant;
        _securitySettings = securitySettings.Value;
    }

    public async Task<TokenResponse> GetTokenAsync(TokenRequest request, string ipAddress, CancellationToken cancellationToken)
    {
        TokenRequestValidator validationRules = new TokenRequestValidator();
        var result = await validationRules.ValidateAsync(request);
        if (result.Errors.Any())
        {
            var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ValidationException(errors, (int)HttpStatusCode.BadRequest);
        }

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

        bool roleExists = await _roleManager.RoleExistsAsync(request.Role);
        if (!roleExists) throw new ValidationException("Role is not found.", (int)HttpStatusCode.BadRequest);

        bool isValidRole = await _userManager.IsInRoleAsync(user, request.Role);
        if (!isValidRole) throw new ValidationException("User is not assigned to this role.", (int)HttpStatusCode.BadRequest);

        var role = await _roleManager.FindByNameAsync(request.Role);
        var roleClaims = await _roleManager.GetClaimsAsync(role!);
        var claimValues = roleClaims.Where(c => c.Type == FSHClaims.Permission)?.Select(a => a.Value).ToList();

        return await GenerateTokensAndUpdateUser(user, request.Role, claimValues!);
    }

    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string role, List<string> claimValues)
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

        return await GenerateTokensAndUpdateUser(user, role, claimValues);
    }

    private async Task<TokenResponse> GenerateTokensAndUpdateUser(ApplicationUser user, string role, List<string> claimValues)
    {
        string token = GenerateJwt(user, role, claimValues);

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

        await _userManager.UpdateAsync(user);

        return new TokenResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime);
    }

    private string GenerateJwt(ApplicationUser user, string role, List<string> claimValues) =>
        GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user, role, claimValues));

    private IEnumerable<Claim> GetClaims(ApplicationUser user, string role, List<string> claimValues)
    {
        var claims = new List<Claim>
        {
            new("id", user.Id),
            new("email", user.Email!),
            new("fullName", $"{user.FirstName} {user.LastName}"),
            new("userName", user.UserName ?? string.Empty),
            new("tenant", _currentTenant!.Id),
            new("userRole", role)
        };

        foreach (string claim in claimValues)
        {
            claims.Add(new Claim("claimValue", claim));
        }

        return claims;
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