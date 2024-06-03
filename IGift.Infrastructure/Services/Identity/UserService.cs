using AutoMapper;
using IGift.Application.Interfaces.Identity;
using IGift.Application.Requests.Identity;
using IGift.Application.Requests.Identity.Users;
using IGift.Application.Responses;
using IGift.Application.Responses.Users;
using IGift.Infrastructure.Models;
using IGift.Shared;
using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IGift.Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<IGiftUser> _userManager;
        private readonly RoleManager<IGiftRole> _roleManager;
        private readonly IMapper _mapper;
        //private readonly IMailService _mailService;
        //private readonly IExcelService _excelService;

        //TODO implementar el mapper
        public UserService(UserManager<IGiftUser> userManager, RoleManager<IGiftRole> roleManager/*, IMapper mapper*/)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            //_mapper = mapper;
        }


        public async Task<IResult> ChangeUserStatus(bool Active, string UserId)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult<string>> ConfirmEmailAsync(string userId, string code)
        {
            throw new NotImplementedException();
        }

        public async Task<string> ExportToExcelAsync(string searchString = "")
        {
            throw new NotImplementedException();
        }

        public async Task<IResult> ForgotPasswordAsync(string Email, string origin)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<List<TokenResponse>>> GetAllAsync()
        {
            //TODO completar
            var users = await _userManager.Users.ToListAsync();
            var result = _mapper.Map<List<TokenResponse>>(users);
            return await Result<List<TokenResponse>>.SuccessAsync(result);
        }

        public async Task<IResult<TokenResponse>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult<UserRolesResponse>> GetRolesAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> HowMany()
        {
            throw new NotImplementedException();
        }

        public async Task<IResult> RegisterAsync(UserCreateRequest model)//TODO este método debería de implementar las configuraciones para verificar Email y talvez la verificación en 2 pasos
        {
            var verification = await VerifyRegistrationUser(model);

            if (!verification.Succeeded)
            {
                return verification;
            }

            var newUser = new IGiftUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName,
                EmailConfirmed = false,
                PhoneNumber = model.PhoneNumber,
                PhoneNumberConfirmed = false,
                CreatedOn = DateTime.Now,
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
            {
                newUser = await _userManager.FindByEmailAsync(model.Email);
                await _userManager.AddToRoleAsync(newUser!, AppConstants.Role.BasicRole);
                return await Result.SuccessAsync("Registro de usuario exitoso");
            }
            return await Result.FailAsync("Error al registrar usuario");
        }

        public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult> UpdateRolesAsync(string UserId, IList<UserRoleModel> UserRoles)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifica si los datos del usuario ya existen
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Succeeded true si no existe, sino un wrapper con el mensaje correspondiente</returns>
        private async Task<IResult> VerifyRegistrationUser(UserCreateRequest model)
        {
            var existsUserName = await _userManager.FindByNameAsync(model.UserName);
            if (existsUserName != null)
            {
                return await Result.FailAsync($"El nombre de usuario{model.UserName} ya esta registrado. Por favor intente con otro");
            }

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                try
                {
                    var existsPhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.PhoneNumber);
                    if (existsPhoneNumber != null)
                    {
                        return await Result.FailAsync($"El número de teléfono ya se encuentra registrado. Por favor intente con otro");
                    }
                }
                catch (Exception e)
                {

                    throw;
                }

            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);
            if (userWithSameEmail != null)
            {
                return await Result.FailAsync("Ya existe una cuenta con el mismo email.Intente loguearse o recuperar la contraseña");
            }

            return await Result.SuccessAsync();
        }

    }
}
