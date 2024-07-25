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
using IGift.Application.Requests.Identity.Users;
using IGift.Application.Requests.Identity.Token;
using IGift.Application.Responses.Identity.Users;

namespace IGift.Infrastructure.Services.Identity
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<IGiftUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IGiftRole> _roleManager;//TODO implementar
        public TokenService(UserManager<IGiftUser> userManager, IConfiguration configuration, RoleManager<IGiftRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }


        /// <summary>
        /// Renueva el tiempo de expiración del token del usuario según el tiempo de expiración del Refresh TokenController. Si este último ya expiró entonces no se podrá renovar el token y el usuario deberá volver a loguearse
        /// </summary>
        /// <param name="tRequest"></param>
        /// <returns></returns>
        public async Task<Result<UserLoginResponse>> RefreshUserToken(TokenRequest tRequest)
        {
            string errorMessage = string.Empty;
            if (tRequest == null) { return await Result<UserLoginResponse>.FailAsync("TokenController nulo"); }

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

            if (!string.IsNullOrEmpty(errorMessage))
            {
                return await Result<UserLoginResponse>.FailAsync(errorMessage);
            }

            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            user.RefreshToken = GenerateRefreshToken();
            await _userManager.UpdateAsync(user);

            //TODO implementar la imagen url
            var response = new UserLoginResponse { Token = token, RefreshToken = user.RefreshToken};

            return await Result<UserLoginResponse>.SuccessAsync(response);
        }

        /// <summary>
        /// Intenta un login con credenciales de usuario. Si es exitoso genera un LoginResponse con un TokenController y un Refresh TokenController cada uno con un tiempo de expiración distinta y otras propiedades que correspondan al usuario como la foto de perfil y la lista de GiftCards
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Result<UserLoginResponse>> LoginAsync(UserLoginRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email!);

            if (user == null)
            {
                return await Result<UserLoginResponse>.FailAsync("Email no encontrado.");
            }

            //TODO: Dejar esto para decidir más adelante si lo usamos o no
            //if (!user.IsActive) 
            //{
            //    return await Result<LoginResponse>.FailAsync("User no activo.Contacte al administrador.");
            //}

            if (!user.EmailConfirmed)
            {
                return await Result<UserLoginResponse>.FailAsync("E-Mail aún no confirmado.");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password!);
            if (!passwordValid)
            {
                return await Result<UserLoginResponse>.FailAsync("Contraseña inválida.");
            }

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(45);
            await _userManager.UpdateAsync(user);

            var response = new UserLoginResponse
            {
                Token = await GenerateJwtAsync(user),
                RefreshToken = user.RefreshToken,
                IdUser = user.Id!
            };

            return await Result<UserLoginResponse>.SuccessAsync(response);
        }

        #region Private


        /// <summary>
        /// Genera un string aleatorio que funciona como RefreshToken
        /// </summary>
        /// <returns></returns>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        /// <summary>
        /// Generación del token en base a un usuario. Este método llama a otros 2 métodos privado
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<string> GenerateJwtAsync(IGiftUser user)
        {
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            return token;
        }
        /// <summary>
        /// Genera un JWT encriptado con los claims personalizados(GetClaimsAsync) y con un tiempo de duración específico
        /// </summary>
        /// <param name="signingCredentials"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
               claims: claims,
               expires: DateTime.UtcNow.AddMinutes(15),
               signingCredentials: signingCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.WriteToken(token);
            return encryptedToken;
        }

        /// <summary>
        /// Devuelve las credenciales de seguridad para generar un JWT
        /// </summary>
        /// <returns></returns>
        private SigningCredentials GetSigningCredentials()
        {
            var Key = Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]!);

            return new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256);
        }

        /// <summary>
        /// Genera una colección de Claims para un token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
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
                new(ClaimTypes.Name,user.FirstName),
                new(ClaimTypes.Surname,user.LastName),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email)
            }.Union(roleClaims);
            // podemos tener claims específicos según el tipo de usuario. Lo mismo que hacían en OliAuto

            return claims;
        }

        /// <summary>
        /// Cuando debemos de renovar el token este método se encarga de devolver los claims del token recibido como parámetro
        /// </summary>
        /// <param name="token">TokenController (no refreshToken) a desencriptar</param>
        /// <returns></returns>
        /// <exception cref="Exception">Claims del usuario</exception>
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var Key = Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]!);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,//Esto se deja en false (por lo menos en este método y no en el program.cs) porque al dejarlo en true si recibimos un token expirado, que es justamente la idea de este método, nos va a arrojar una excepcion de tipo TokenExpiredException
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

        #endregion
    }
}
