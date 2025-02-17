﻿using IGift.Application.Responses.Peticiones;
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
            await Clients.All.SendAsync(AppConstants.SignalR.ConnectUser, userId);
        }

        public async Task OnDisconnectAsync(string userId)
        {
            await Clients.All.SendAsync(AppConstants.SignalR.DisconnectUser, userId);
        }

        public async Task ChatNotificationAsync(string message, string receiverUserId, string senderUserId)
        {
            await Clients.User(receiverUserId).SendAsync(AppConstants.SignalR.ReceiveChatNotification, message, receiverUserId, senderUserId);
        }

        public async Task SendShopCartNotificationAsync(PeticionesResponse p, string UserId)
        {
            await Clients.User(UserId).SendAsync(AppConstants.SignalR.ReceiveShopCartNotificationAsync, p);
        }

        public async Task RegenerateTokensAsync()
        {
            await Clients.All.SendAsync(AppConstants.SignalR.ReceiveRegenerateTokens);
        }

        //public async Task SendMessageAsync(Chat chat, string userName)
        //{
        //    //await Clients.All.SendAsync("ReceiveMessage", userName);
        //    await Clients.User(chat.ToUserId).SendAsync(AppConstants.SignalR.ReceiveMessage, chat, userName);
        //    await Clients.User(chat.FromUserId).SendAsync(AppConstants.SignalR.ReceiveMessage, chat, userName);
        //}
    }
}
