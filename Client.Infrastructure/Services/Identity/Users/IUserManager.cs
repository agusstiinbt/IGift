using IGift.Application.Responses.Identity.Users;
using IGift.Shared.Wrapper;

namespace Client.Infrastructure.Services.Identity.Users
{
    public interface IUserManager
    {
        Task<IResult<List<UserResponse>>> GetUsersAsync();
        Task<IResult<UserResponse>> GetUserById(string UserId);
    }
}
