using IGift.Application.Requests.Identity;
using IGift.Application.Responses;
using IGift.Shared.Wrapper;

namespace IGift.Application.Interfaces.Identity
{
    public interface IUserService
    {
        Task<Result<List<ApplicationUserResponse>>> GetAllAsync();
        Task<int> HowMany();

        Task<IResult<ApplicationUserResponse>> GetByIdAsync(int id);//fijarse si está bien poner un int Id
        Task<IResult> RegisterAsync(ApplicationUserRequest model);

        Task<IResult> ChangeUserStatus(bool Active, string UserId);

        Task<IResult<UserRolesResponse>> GetRolesAsync(string id);

        Task<IResult> UpdateRolesAsync(string UserId,IList<UserRoleModel>UserRoles);

        Task<IResult<string>> ConfirmEmailAsync(string userId, string code);

        Task<IResult> ForgotPasswordAsync(string Email, string origin);
        Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);

        Task<string> ExportToExcelAsync(string searchString = "");
    }
}
