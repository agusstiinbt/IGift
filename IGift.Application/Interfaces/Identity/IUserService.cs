﻿using IGift.Application.Requests.Identity;
using IGift.Application.Requests.Identity.Users;
using IGift.Application.Responses;
using IGift.Application.Responses.Users;
using IGift.Shared.Wrapper;

namespace IGift.Application.Interfaces.Identity
{
    public interface IUserService
    {
        Task<Result<List<TokenResponse>>> GetAllAsync();
        Task<int> HowMany();

        Task<IResult<TokenResponse>> GetByIdAsync(int id);//fijarse si está bien poner un int Id
        Task<IResult> RegisterAsync(UserCreateRequest model);

        Task<IResult> ChangeUserStatus(bool Active, string UserId);

        Task<IResult<UserRolesResponse>> GetRolesAsync(string id);

        Task<IResult> UpdateRolesAsync(string UserId,IList<UserRoleModel>UserRoles);

        Task<IResult<string>> ConfirmEmailAsync(string userId, string code);

        Task<IResult> ForgotPasswordAsync(string Email, string origin);
        Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);

        Task<string> ExportToExcelAsync(string searchString = "");
    }
}
