using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Interfaces.Chat;
using IGift.Application.Models.Chat;
using IGift.Shared.Wrapper;

namespace IGift.Application.Interfaces.Communication.Chat
{
    public interface IChatService
    {
        Task<IResult> SaveMessage(ChatHistory<IChatUser> message);
        Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryByIdAsync(string chatId);
    }
}
