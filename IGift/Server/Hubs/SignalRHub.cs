using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Responses.Peticiones;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IGift.Server.Hubs
{
    [Authorize]
    public class SignalRHub : Hub
    {
        public async Task OnConnectAsync(string userId)
        {
            await Clients.All.SendAsync(AppConstants.SignalR.ConnectUserAsync, userId);
        }

        public async Task OnDisconnectAsync(string userId)
        {
            await Clients.All.SendAsync(AppConstants.SignalR.DisconnectUserAsync, userId);
        }

        /// <summary>
        /// Actualiza los ultimos mensajes de un chat
        /// </summary>
        /// <param name="message"></param>
        /// <param name="receiverUserId"></param>
        /// <param name="senderUserId"></param>
        /// <returns></returns>
        public async Task SendChatMessageAsync(ChatHistoryResponse chatHistory)
        {
            await Clients.User(chatHistory.ToUserId).SendAsync(AppConstants.SignalR.ReceiveChatMessageAsync, chatHistory);
        }

        public async Task SendShopCartNotificationAsync(PeticionesResponse p, string UserId)
        {
            await Clients.User(UserId).SendAsync(AppConstants.SignalR.ReceiveShopCartNotificationAsync, p);
        }

        public async Task RegenerateTokensAsync()
        {
            await Clients.All.SendAsync(AppConstants.SignalR.ReceiveRegenerateTokensAsync);
        }

        /// <summary>
        /// Este metodo se encarga de notificar con un SnackBar con el mensaje de "llego un mensaje nuevo"
        /// </summary>
        /// <param name="chat"></param>
        /// <returns></returns>
        public async Task SendChatNotificationAsync(ChatHistoryResponse chat)
        {
            await Clients.User(chat.ToUserId).SendAsync(AppConstants.SignalR.ReceiveChatNotificationAsync, chat);
        }

        //TODO revisar esto
        public async Task SetLastMessageToSeen(string ToUserId)
        {
            await Clients.User(ToUserId).SendAsync(AppConstants.SignalR.SetLastMessageToSeen, ToUserId);
        }
    }
}
