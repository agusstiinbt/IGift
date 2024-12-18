using IGift.Application.Models.Chat;
using IGift.Shared.Wrapper;

namespace IGift.Application.Interfaces.Communication.Chat
{
    public interface IChatService
    {
        Task<IResult> SaveMessage(ChatHistory chatHistory);
        Task<IResult<List<ChatHistory>>> GetChatHistoryByIdAsync(string chatId);
    }
}
