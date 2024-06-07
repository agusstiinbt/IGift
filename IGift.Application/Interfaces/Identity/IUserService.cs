﻿using IGift.Application.Requests.Identity;
using IGift.Application.Requests.Identity.Password;
using IGift.Application.Requests.Identity.Users;
using IGift.Application.Responses;
using IGift.Application.Responses.Identity.Users;
using IGift.Application.Responses.Users;
using IGift.Shared.Wrapper;

namespace IGift.Application.Interfaces.Identity
{
    public interface IUserService
    {
        Task<Result<List<UserResponse>>> GetAllAsync();
        Task<int> HowMany();

        Task<IResult<UserResponse>> GetByIdAsync(string id);
        Task<IResult> RegisterAsync(UserCreateRequest model);

        Task<IResult> ChangeUserStatus(ChangeUserRequest request);

        Task<IResult<UserRolesResponse>> GetRolesAsync(string id);

        /// <summary>
        /// La idea de este método es que el usuario actual pueda modificar los roles de un usuario determinado. Esto solo funciona si somos adminitradores
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request);

        Task<IResult<string>> ConfirmEmailAsync(string userId, string code);

        Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);
        Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);

        Task<string> ExportToExcelAsync(string searchString = "");
    }
}
