using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ICISAdminPortal.Application.Common.Exceptions;
using ICISAdminPortal.Application.Identity.Tokens;
using ICISAdminPortal.Infrastructure.Auth;
using ICISAdminPortal.Infrastructure.Auth.Jwt;
using ICISAdminPortal.Infrastructure.Multitenancy;
using ICISAdminPortal.Shared.Authorization;
using ICISAdminPortal.Shared.Multitenancy;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Net;

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
<<<<<<< HEAD
            throw new UnauthorizedException(_t["Authentication Failed."], (int)HttpStatusCode.BadRequest);
=======
            throw new Application.Exceptions.ValidationException(_t["Authentication Failed."], (int)HttpStatusCode.BadRequest);
>>>>>>> Fix/Migrations
        }

        if (!user.IsActive)
        {
<<<<<<< HEAD
            throw new UnauthorizedException(_t["User Not Active. Please contact the administrator."], (int)HttpStatusCode.BadRequest);
=======
            throw new Application.Exceptions.ValidationException(_t["User Not Active. Please contact the administrator."], (int)HttpStatusCode.BadRequest);
>>>>>>> Fix/Migrations
        }

        if (_securitySettings.RequireConfirmedAccount && !user.EmailConfirmed)
        {
<<<<<<< HEAD
            throw new UnauthorizedException(_t["E-Mail not confirmed."], (int)HttpStatusCode.BadRequest);
=======
            throw new Application.Exceptions.ValidationException(_t["E-Mail not confirmed."], (int)HttpStatusCode.BadRequest);
>>>>>>> Fix/Migrations
        }

        if (_currentTenant.Id != MultitenancyConstants.Root.Id)
        {
            if (!_currentTenant.IsActive)
            {
<<<<<<< HEAD
                throw new UnauthorizedException(_t["Tenant is not Active. Please contact the Application Administrator."], (int)HttpStatusCode.BadRequest);
=======
                throw new Application.Exceptions.ValidationException(_t["Tenant is not Active. Please contact the Application Administrator."], (int)HttpStatusCode.BadRequest);
>>>>>>> Fix/Migrations
            }

            if (DateTime.UtcNow > _currentTenant.ValidUpto)
            {
<<<<<<< HEAD
                throw new UnauthorizedException(_t["Tenant Validity Has Expired. Please contact the Application Administrator."], (int)HttpStatusCode.BadRequest);
=======
                throw new Application.Exceptions.ValidationException(_t["Tenant Validity Has Expired. Please contact the Application Administrator."], (int)HttpStatusCode.BadRequest);
>>>>>>> Fix/Migrations
            }
        }

        return await GenerateTokensAndUpdateUser(user, ipAddress);
    }

    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress)
    {
        var userPrincipal = GetPrincipalFromExpiredToken(request.Token);
        string? userEmail = userPrincipal.GetEmail();
        var user = await _userManager.FindByEmailAsync(userEmail!);
        if (user is null)
        {
<<<<<<< HEAD
            throw new UnauthorizedException(_t["Authentication Failed."], (int)HttpStatusCode.BadRequest);
=======
            throw new Application.Exceptions.ValidationException(_t["Authentication Failed."], (int)HttpStatusCode.BadRequest);
>>>>>>> Fix/Migrations
        }

        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
<<<<<<< HEAD
            throw new UnauthorizedException(_t["Invalid Refresh Token."], (int)HttpStatusCode.BadRequest);
=======
            throw new Application.Exceptions.ValidationException(_t["Invalid Refresh Token."], (int)HttpStatusCode.BadRequest);
>>>>>>> Fix/Migrations
        }

        return await GenerateTokensAndUpdateUser(user, ipAddress);
    }

    private async Task<TokenResponse> GenerateTokensAndUpdateUser(ApplicationUser user, string ipAddress)
    {
        string token = GenerateJwt(user, ipAddress);

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

        await _userManager.UpdateAsync(user);

        return new TokenResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime);
    }

    private string GenerateJwt(ApplicationUser user, string ipAddress) =>
        GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user, ipAddress));

    private IEnumerable<Claim> GetClaims(ApplicationUser user, string ipAddress) =>
        new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(FSHClaims.Fullname, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Name, user.FirstName ?? string.Empty),
            new(ClaimTypes.Surname, user.LastName ?? string.Empty),
            new(FSHClaims.IpAddress, ipAddress),
            new(FSHClaims.Tenant, _currentTenant!.Id),
            new(FSHClaims.ImageUrl, user.ImageUrl ?? string.Empty),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
        };

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
<<<<<<< HEAD
            throw new UnauthorizedException(_t["Invalid Token."], (int)HttpStatusCode.Unauthorized);
=======
            throw new Application.Exceptions.ValidationException(_t["Invalid Token."], (int)HttpStatusCode.BadRequest);
>>>>>>> Fix/Migrations
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
}