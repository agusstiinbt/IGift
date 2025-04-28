namespace IGift.Shared.Constants
{
    public static partial class AppConstants
    {
        public static class SignalR
        {
            public const string HubUrl = "/signalRHub";

            public const string UpdateDashboardAsync = "UpdateDashboardAsync";
            public const string ReceiveUpdateDashboardAsync = "ReceiveUpdateDashboardAsync";

            public const string RegenerateTokensAsync = "RegenerateTokensAsync";
            public const string ReceiveRegenerateTokensAsync = "ReceiveRegenerateTokensAsync";

            //Chat
            public const string ReceiveChatNotificationAsync = "ReceiveChatNotificationAsync";
            public const string SendChatNotificationAsync = "SendChatNotificationAsync";

            public const string ReceiveChatMessageAsync = "ReceiveChatMessageAsync";
            public const string SendChatMessageAsync = "SendChatMessageAsync";



            public const string ReceiveShopCartNotificationAsync = "ReceiveShopCartNotificationAsync";
            public const string SendShopCartNotificationAsync = "SendShopCartNotificationAsync";

  



            public const string OnConnect = "OnConnectAsync";
            public const string ConnectUserAsync = "ConnectUserAsync";

            public const string OnDisconnectAsync = "OnDisconnectAsync";
            public const string DisconnectUserAsync = "DisconnectUserAsync";

            public const string OnChangeRolePermissions = "OnChangeRolePermissions";
            public const string LogoutUsersByRoleAsync = "LogoutUsersByRoleAsync";

            /// <summary>
            /// Envía la petición para dejar a un usuario online/offline
            /// </summary>
            public const string PingRequest = "PingRequestAsync";
            /// <summary>
            /// Recibe la petición para dejar a un usuario online/offline
            /// </summary>
            public const string PingResponse = "PingResponseAsync";
        }
    }
}