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

        public async Task<Result<ApplicationUserResponse>> GetRefreshToken(UserLoginRequest model)//TODO investigar para qué usa un refreshToken
        {
            throw new NotImplementedException();
        }

        public async Task<Result<ApplicationUserResponse>> LoginAsync(UserLoginRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email!);

            if (user == null)
            {
                return await Result<ApplicationUserResponse>.FailAsync("Email no encontrado.");
            }

            //TODO: Dejar esto para decidir más adelante si lo usamos o no
            //if (!user.IsActive) 
            //{
            //    return await Result<ApplicationUserResponse>.FailAsync("User no activo.Contacte al administrador.");
            //}

            if (!user.EmailConfirmed)
            {
                return await Result<ApplicationUserResponse>.FailAsync("E-Mail aún no confirmado.");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password!);
            if (!passwordValid)
            {
                return await Result<ApplicationUserResponse>.FailAsync("Contraseña inválida.");
            }

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            var response = new ApplicationUserResponse
            {
                Token = await GenerateJwtAsync(user),
                RefreshToken = user.RefreshToken,
                UserImageURL = user.ProfilePictureDataUrl
            };

            return await Result<ApplicationUserResponse>.SuccessAsync(response);
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
               expires: DateTime.UtcNow.AddDays(2),
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
    }
}
