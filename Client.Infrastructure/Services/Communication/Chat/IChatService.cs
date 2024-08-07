using IGift.Application.Models.Chat;
using IGift.Shared.Wrapper;

namespace IGift.Client.Infrastructure.Services.Communication.Chat
{
    public interface IChatService
    {
        Task<IResult> SaveMessage(ChatHistory chatHistory);
    }
}
