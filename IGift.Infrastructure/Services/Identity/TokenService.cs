using IGift.Application.Responses;
using IGift.Infrastructure.Models;
using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IGift.Application.Interfaces.Identity;
using IGift.Application.Requests.Identity;

namespace IGift.Infrastructure.Services.Identity
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<IGiftUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IGiftRole> _roleManager;
        public TokenService(UserManager<IGiftUser> userManager, IConfiguration configuration, RoleManager<IGiftRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        public async Task<Result<TokenResponse>> GetRefreshTokenAsync(TokenRequest tRequest)
        {
            string errorMessage = string.Empty;
            if (tRequest == null) { return await Result<TokenResponse>.FailAsync("Token nulo"); }

            var userPrincipal = GetPrincipalFromExpiredToken(tRequest.Token);
            var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail!);

            if (user == null)
            {
                errorMessage = "Usuario no encontrado";
            }
            if (user.RefreshToken != tRequest.RefreshToken)
            {
                errorMessage = "token invalido";
            }
            if (user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                errorMessage = "El refresh token ya ha expirado";
            }

            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            user.RefreshToken = GenerateRefreshToken();
            await _userManager.UpdateAsync(user);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return await Result<TokenResponse>.FailAsync(errorMessage);
            }

            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };

            return await Result<TokenResponse>.SuccessAsync(response);
        }

        public async Task<Result<TokenResponse>> LoginAsync(UserLoginRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email!);

            if (user == null)
            {
                return await Result<TokenResponse>.FailAsync("Email no encontrado.");
            }

            //TODO: Dejar esto para decidir más adelante si lo usamos o no
            //if (!user.IsActive) 
            //{
            //    return await Result<TokenResponse>.FailAsync("User no activo.Contacte al administrador.");
            //}

            if (!user.EmailConfirmed)
            {
                return await Result<TokenResponse>.FailAsync("E-Mail aún no confirmado.");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password!);
            if (!passwordValid)
            {
                return await Result<TokenResponse>.FailAsync("Contraseña inválida.");
            }

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(3);
            await _userManager.UpdateAsync(user);

            var response = new TokenResponse
            {
                Token = await GenerateJwtAsync(user),
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                UserImageURL = user.ProfilePictureDataUrl
            };

            return await Result<TokenResponse>.SuccessAsync(response);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateJwtAsync(IGiftUser user)
        {
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            return token;
        }

        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
               claims: claims,
               //expires: DateTime.UtcNow.AddDays(2),
               expires: DateTime.Now.AddMinutes(1),
               signingCredentials: signingCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.WriteToken(token);
            return encryptedToken;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var Key = Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]!);

            return new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256);
        }

        private async Task<IEnumerable<Claim>> GetClaimsAsync(IGiftUser user)
        {
            //TODO si vamos a hacer uso de los 'Permissions' fijarse el código de blazorHero
            //var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            // var permissionClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
                //var thisRole = await _roleManager.FindByNameAsync(role);
                // var allPermissionsForThisRoles = await _roleManager.GetClaimsAsync(thisRole);
                //  permissionClaims.AddRange(allPermissionsForThisRoles);
            }
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email)
            }.Union(roleClaims);
            // podemos tener claims específicos según el tipo de usuario. Lo mismo que hacían en OliAuto

            return claims;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var Key = Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]!);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("token invalido");
            }

            return principal;

        }
    }
}
