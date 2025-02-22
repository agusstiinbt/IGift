using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Models.Chat;
using IGift.Shared.Wrapper;

namespace IGift.Client.Infrastructure.Services.Communication.Chat
{
    public class ChatManager : IChatManager
    {
        public Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryByIdAsync(string chatId)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<IEnumerable<ChatUser>>> LoadChatUsers(string CurrentUserId)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> SaveMessage(SaveChatMessage saveChatMessage)
        {
            throw new NotImplementedException();
        }
    }
}
