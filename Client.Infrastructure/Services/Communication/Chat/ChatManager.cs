using System.Net.Http.Json;
using Client.Infrastructure.Extensions;
using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Models.Chat;
using IGift.Shared.Constants.Apis;
using IGift.Shared.Wrapper;
using Microsoft.Extensions.Logging;

namespace IGift.Client.Infrastructure.Services.Communication.Chat
{
    public class ChatManager : IChatManager
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ChatManager> _logger;

        public ChatManager(HttpClient httpClient, ILogger<ChatManager> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatById(SearchChatById obj)
        {
            var response = await _httpClient.PostAsJsonAsync(ChatController.GetChatById, obj);
            var result = await response.ToResult<IEnumerable<ChatHistoryResponse>>();
            return result;
        }

        public async Task<IResult<IEnumerable<ChatUserResponse>>> LoadChatUsers(LoadChatUsers obj)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ChatController.LoadChatUsers, obj);
                var result = await response.ToResult<IEnumerable<ChatUserResponse>>();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("Error al cargar los chats del usuario. Mensaje: {Message} MENSAJE " + e.Message.ToString() + " INNER Exception: " + e.InnerException.ToString());
                return await Result<IEnumerable<ChatUserResponse>>.FailAsync("Error al cargar los chats del usuario. Mensaje: {Message} MENSAJE \" + e.Message.ToString() + \" INNER Exception: \" + e.InnerException.ToString()");
            }
        }


        public async Task<IResult<IEnumerable<ChatUserResponse>>> SaveMessage(SaveChatMessage saveChatMessage)
        {
            var response = await _httpClient.PostAsJsonAsync(ChatController.SaveMessage, saveChatMessage);
            var result = await response.ToResult<IEnumerable<ChatUserResponse>>();
            return result;
        }
    }
}
