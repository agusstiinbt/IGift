using System.Net.Http.Json;
using Client.Infrastructure.Extensions;
using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Models.Chat;
using IGift.Shared.Constants.Apis;
using IGift.Shared.Wrapper;

namespace IGift.Client.Infrastructure.Services.Communication.Chat
{
    public class ChatManager : IChatManager
    {
        private readonly HttpClient _httpClient;

        public ChatManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatById(GetChatById obj)
        {
            var response = await _httpClient.PostAsJsonAsync(ChatController.GetChatById, obj);
            var result = await response.ToResult<IEnumerable<ChatHistoryResponse>>();
            return result;
        }

        public async Task<IResult<IEnumerable<ChatUserResponse>>> LoadChatUsers(LoadChatUsers obj)
        {
            var response = await _httpClient.PostAsJsonAsync(ChatController.LoadChatUsers, obj);
            var result = await response.ToResult<IEnumerable<ChatUserResponse>>();
            return result;
        }

        public async Task<IResult> SaveMessage(SaveChatMessage saveChatMessage)
        {
            var response = await _httpClient.PostAsJsonAsync(ChatController.SaveMessage, saveChatMessage);
            var result = await response.ToResult<SaveChatMessage>();
            return result;
        }
    }
}
