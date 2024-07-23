using IGift.Application.Models.Chat;
using IGift.Shared;
using Microsoft.AspNetCore.SignalR;

namespace IGift.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessageAsync(Chat chat, string userName)
        {
            //await Clients.All.SendAsync("ReceiveMessage", userName);
            await Clients.User(chat.ToUserId).SendAsync(AppConstants.SignalR.ReceiveMessage, chat, userName);
            await Clients.User(chat.FromUserId).SendAsync(AppConstants.SignalR.ReceiveMessage, chat, userName);
        }

        public async Task ChatNotificationAsync(string message, string receiverUserId, string senderUserId)
        {
            await Clients.User(receiverUserId).SendAsync(AppConstants.SignalR.ReceiveChatNotification, message, receiverUserId, senderUserId);
        }
    }
}
