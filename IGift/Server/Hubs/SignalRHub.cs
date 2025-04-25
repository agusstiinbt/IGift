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

        public async Task ChatNotificationAsync(string message, string receiverUserId, string senderUserId)
        {
            await Clients.User(receiverUserId).SendAsync(AppConstants.SignalR.ReceiveChatNotificationAsync, message, receiverUserId, senderUserId);
        }

        public async Task SendShopCartNotificationAsync(PeticionesResponse p, string UserId)
        {
            await Clients.User(UserId).SendAsync(AppConstants.SignalR.ReceiveShopCartNotificationAsync, p);
        }

        public async Task RegenerateTokensAsync()
        {
            await Clients.All.SendAsync(AppConstants.SignalR.ReceiveRegenerateTokensAsync);
        }

        public async Task SendMessageAsync(ChatHistoryResponse chatHistory)
        {
            //await Clients.All.SendAsync(AppConstants.SignalR.ReceiveMessageAsync, chatHistory);
            await Clients.User(chatHistory.ToUserId).SendAsync(AppConstants.SignalR.ReceiveMessageAsync, chatHistory);
        }

        public async Task SendChatNotificationAsync(SaveChatMessage chat, string receiverUserId)
        {
            await Clients.User(chat.ToUserId).SendAsync(AppConstants.SignalR.ReceiveChatNotificationAsync, chat, receiverUserId);
        }
    }
}
