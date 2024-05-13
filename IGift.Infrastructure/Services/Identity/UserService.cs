using AutoMapper;
using IGift.Application.Interfaces.Identity;
using IGift.Application.Requests.Identity;
using IGift.Application.Responses;
using IGift.Infrastructure.Models;
using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IGift.Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationUserRole> _roleManager;
        //private readonly IMailService _mailService;
        //private readonly IExcelService _excelService;
        private readonly IMapper _mapper;
      

        public UserService(UserManager<ApplicationUser> userManager/*, RoleManager<ApplicationUserRole> roleManager/*, IMapper mapper*/)
        {
            _userManager = userManager;
            //_roleManager = roleManager;
            //_mapper = mapper;
        }

        //public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationUserRole> roleManager,IMapper mapper)
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

        public Task<IResult> RegisterAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> UpdateRolesAsync(string UserId, IList<UserRoleModel> UserRoles)
        {
            throw new NotImplementedException();
        }
    }
}
