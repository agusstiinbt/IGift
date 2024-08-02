using IGift.Application.Models.Chat;
using IGift.Shared.Wrapper;

namespace IGift.Application.Interfaces.Chat.Service
{
    public interface IChatService
    {
        Task<IResult> SaveMessage<TUser>(ChatHistory<TUser> chatHistory, string userId) where TUser : IChatUser;
    }

}
