namespace IGift.Shared.Constants
{
    public static partial class AppConstants
    {
        public static class SignalR
        {
            public const string HubUrl = "/signalRHub";

            public const string SendUpdateDashboard = "UpdateDashboardAsync";
            public const string ReceiveUpdateDashboard = "UpdateDashboard";

            public const string SendRegenerateTokens = "RegenerateTokensAsync";
            public const string ReceiveRegenerateTokens = "RegenerateTokens";

            public const string ReceiveChatNotification = "ReceiveChatNotification";
            public const string SendChatNotification = "ChatNotificationAsync";

            public const string ReceiveShopCartNotificationAsync = "ReceiveShopCartNotificationAsync";
            public const string SendShopCartNotificationAsync = "SendShopCartNotificationAsync";

            public const string ReceiveMessage = "ReceiveMessage";
            public const string SendMessage = "SendMessageAsync";

            public const string OnConnect = "OnConnectAsync";
            public const string ConnectUser = "ConnectUser";

            public const string OnDisconnect = "OnDisconnectAsync";
            public const string DisconnectUser = "DisconnectUser";

            public const string OnChangeRolePermissions = "OnChangeRolePermissions";
            public const string LogoutUsersByRole = "LogoutUsersByRole";

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
