using AutoMapper;
using IGift.Application.Interfaces.Identity;
using IGift.Application.Requests.Identity;
using IGift.Application.Responses;
using IGift.Infrastructure.Models;
using IGift.Shared.Role;
using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IGift.Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<IGiftUser> _userManager;
        private readonly RoleManager<IGiftRole> _roleManager;
        //private readonly IMailService _mailService;
        //private readonly IExcelService _excelService;
        private readonly IMapper _mapper;


        public UserService(UserManager<IGiftUser> userManager/*, RoleManager<IGiftRole> roleManager/*, IMapper mapper*/)
        {
            _userManager = userManager;
            //_roleManager = roleManager;
            //_mapper = mapper;
        }

        //public UserService(UserManager<IGiftUser> userManager, RoleManager<IGiftRole> roleManager,IMapper mapper)
        //{
        //    _userManager = userManager;
        //    _roleManager = roleManager;
        //    _mapper = mapper;
        //}

        public Task<IResult> ChangeUserStatus(bool Active, string UserId)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<string>> ConfirmEmailAsync(string userId, string code)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportToExcelAsync(string searchString = "")
        {
            throw new NotImplementedException();
        }

        public Task<IResult> ForgotPasswordAsync(string Email, string origin)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<List<ApplicationUserResponse>>> GetAllAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = _mapper.Map<List<ApplicationUserResponse>>(users);
            return await Result<List<ApplicationUserResponse>>.SuccessAsync(result);
        }

        public Task<IResult<ApplicationUserResponse>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<UserRolesResponse>> GetRolesAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<int> HowMany()
        {
            throw new NotImplementedException();
        }

        public async Task<IResult> RegisterAsync(ApplicationUserRequest model)//TODO este método debería de implementar las configuraciones para verificar Email y talvez la verificación en 2 pasos
        {
            //var newUser = new IGiftUser { UserName = model.UserName, Email = model.Email, CreatedOn=DateTime.Now };
            var verification = await VerifyRegistrationUser(model);
            if (!verification.Succeeded)
            {
                return verification;
            }

            var user = new IGiftUser
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                PhoneNumber = model.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                user = await _userManager.FindByEmailAsync(user.Email);
                await _userManager.AddToRoleAsync(user, RoleConstants.BasicRole);
            }
            return null;
        }

        public Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> UpdateRolesAsync(string UserId, IList<UserRoleModel> UserRoles)
        {
            throw new NotImplementedException();
        }

        private async Task<IResult> VerifyRegistrationUser(ApplicationUserRequest model)
        {
            var existsUserName = await _userManager.FindByNameAsync(model.UserName);
            if (existsUserName != null)
            {
                return await Result.FailAsync($"El nombre de usuario{model.UserName} ya esta registrado. Por favor intente con otro");
            }

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                var existsPhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.PhoneNumber);
                if (existsPhoneNumber != null)
                {
                    return await Result.FailAsync($"El número de teléfono ya se encuentra registrado. Por favor intente con otro");
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
