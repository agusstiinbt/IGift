using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Models.Chat;
using IGift.Shared.Wrapper;

namespace IGift.Client.Infrastructure.Services.Communication.Chat
{
    public interface IChatManager
    {
        Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryByIdAsync(string chatId);

        Task<IResult<IEnumerable<ChatUser>>> LoadChatUsers(string CurrentUserId);

        Task<IResult> SaveMessage(SaveChatMessage saveChatMessage);
    }
}
