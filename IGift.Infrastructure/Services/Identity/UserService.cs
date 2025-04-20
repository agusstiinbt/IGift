using System.Text;
using System.Text.Encodings.Web;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hangfire;
using IGift.Application.CQRS.Identity.Email;
using IGift.Application.CQRS.Identity.Password;
using IGift.Application.CQRS.Identity.Users;
using IGift.Application.Interfaces.Communication.Mail;
using IGift.Application.Interfaces.Identity;
using IGift.Application.Responses.Identity.Users;
using IGift.Application.Responses.Users;
using IGift.Infrastructure.Models;
using IGift.Shared.Constants;
using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace IGift.Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<IGiftUser> _userManager;
        private readonly RoleManager<IGiftRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        //private readonly IExcelService _excelService;

        public UserService(UserManager<IGiftUser> userManager, RoleManager<IGiftRole> roleManager, IMapper mapper, IMailService mailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _mailService = mailService;
        }

        public async Task<IResult> ChangeUserStatus(ChangeUserRequest request)
        {
            var user = await _userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync();
            var isAdmin = await _userManager.IsInRoleAsync(user, AppConstants.Role.AdministratorRole);
            if (isAdmin)
            {
                return await Result.FailAsync("No se permite modificar el estado de perfil de un Administrador");
            }
            if (user != null)
            {
                user.IsActive = request.ActivateUser;
                var identityResult = await _userManager.UpdateAsync(user);
            }
            return await Result.SuccessAsync();
        }

        public async Task<IResult<string>> ConfirmEmailAsync(string userId, string code)
        {
            // para esto hay que leer la documentación de microsoft. NO basarse en el código de blazorHero 
            throw new NotImplementedException();
        }

        public async Task<string> ExportToExcelAsync(string searchString = "")
        {
            throw new NotImplementedException();
        }

        public async Task<Result<IEnumerable<UserResponse>>> GetAllAsync() => await Result<IEnumerable<UserResponse>>.SuccessAsync(await _userManager.Users.ProjectTo<UserResponse>(_mapper.ConfigurationProvider).ToListAsync());
        //Código más completo =>
        //{
        //    // Result<List<UserResponse>>.SuccessAsync(

        //    var response = await _userManager.Users.ProjectTo<UserResponse>(_mapper.ConfigurationProvider).ToListAsync();
        //    return await Result<List<UserResponse>>.SuccessAsync(response);
        //}

        public async Task<IResult<UserResponse>> GetByIdAsync(string id)
        {
            var response = await _userManager.FindByIdAsync(id);

            if (response == null)
                return await Result<UserResponse>.FailAsync();
            //TODO mejorar el mapeo u alguna otra cosa para hacer

            var result = new UserResponse()
            {
                Id = id,
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                CreatedOn = response.CreatedOn,
                ProfilePictureDataUrl = response.ProfilePictureDataUrl
            };

            return await Result<UserResponse>.SuccessAsync(result);
        }

        public async Task<IResult<UserRolesResponse>> GetRolesAsync(string id)
        {
            var list = new List<UserRoleModel>();

            var user = await _userManager.FindByIdAsync(id);//Nos traemos el usuario correspondiente
            var roles = await _roleManager.Roles.ToListAsync();//Traemos todos los roles (por default son 2, véase databaSeeder)

            foreach (var role in roles)
            {
                var userRolesViewModel = new UserRoleModel
                {
                    RoleName = role.Name,
                    RoleDescription = role.Description
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))   //Podemos traernos la lista completa de usuarios y de roles, pero con esto verificamos si un usuario tiene un rol específico 
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }

                list.Add(userRolesViewModel);
            }
            var result = new UserRolesResponse { UserRoles = list };
            return await Result<UserRolesResponse>.SuccessAsync(result);
        }

        public async Task<int> HowMany() => await _userManager.Users.CountAsync();

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
                // PhoneNumber = model.PhoneNumber,
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

        public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return await Result.FailAsync("El usuario no existe o no se encuentra confirmado");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "account/reset-password";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            var passwordResetURL = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);
            var emailRequest = new MailRequest
            {
                Body = string.Format("Por favor, cambie su contraseña haciendo click <a href='{0}'>AQUÍ</a>.", HtmlEncoder.Default.Encode(passwordResetURL)),
                Subject = "Reset Password",
                To = request.Email
            };
            BackgroundJob.Enqueue(() => _mailService.SendAsync(emailRequest));
            return await Result.SuccessAsync("El correo para reestablecer la contraseña ha sido enviado a su correo");
        }

        public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return await Result.FailAsync("El usuario no existe");
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.ConfirmedNewPassword);
            if (result.Succeeded)
            {
                return await Result.SuccessAsync("Cambio de clave exitoso");
            }
            else
            {
                return await Result.FailAsync("Ha ocurrido un problema al intentar cambiar clave");
            }
        }

        public async Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user.Email == AppConstants.Server.AdminEmail)
            {
                return await Result.FailAsync("No se permite cambiar el rol a este usuario");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var selectedRoles = request.UserRoles.Where(x => x.Selected).ToList();

            if (request.CurrentUserRole != AppConstants.Role.AdministratorRole)
            {
                return await Result.FailAsync("Solo el administrador puede modificar o agregar roles a otros usuarios");
            }

            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            result = await _userManager.AddToRolesAsync(user, selectedRoles.Select(y => y.RoleName));
            return await Result.SuccessAsync("Roles actualizados");
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

            //if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            //{
            //    try
            //    {
            //        var existsPhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.PhoneNumber);
            //        if (existsPhoneNumber != null)
            //        {
            //            return await Result.FailAsync($"El número de teléfono ya se encuentra registrado. Por favor intente con otro");
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        throw new Exception("Error al verificar registro de usuario: " + e.Message);
            //    }

            //}

            var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);
            if (userWithSameEmail != null)
            {
                return await Result.FailAsync("Ya existe una cuenta con el mismo email.Intente loguearse o recuperar la contraseña");
            }

            return await Result.SuccessAsync();
        }
    }
}
