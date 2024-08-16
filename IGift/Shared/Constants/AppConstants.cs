namespace IGift.Shared
{
    public static class AppConstants
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

        public static class Role
        {
            public const string AdministratorRole = "Administrator";
            public const string BasicRole = "Basic";
        }

        public static class Controllers
        {
            public static class Users
            {
                private static string route = "api/Users/";

                public static string GetAll = route + "GetAll";
                public static string Register = route + "Register";
                public static string GetById = route + "GetById";
                public static string ChangeUserStatus = route + "ChangeUserStatus";
                public static string ForgotPassword = route + "ForgotPassword";
                public static string GetRolesFromUserId = route + "GetRolesFromUserId";
                public static string UpdateRolesFromUser = route + "UpdateRolesFromUser";
            }

            public static class TokenController
            {
                private static string route = "api/Token/";
                public static string LogIn = route + "Login";
                public static string RefreshToken = route + "RefreshToken";
            }

            public static class RolesController
            {
                private static string route = "api/Roles/";
            }

            public static class NotificationController
            {
                private static string route = "api/Notification";
                public static string GetAll = route;
            }

            public static class PedidosController
            {
                private static string route = "api/Pedidos/";

                public static string GetAll = route + "GetAll";
            }

            public static class PeticionesController
            {
                private static string route = "api/Peticiones";
                public static string GetAll = route;
            }

            public static class FilesController
            {
                private static string route = "api/Files/";

                public static string GetProfilePictureById = route + "GetProfilePictureById";
                public static string UploadProfilePicture = route + "UploadProfilePicture";
            }
        }

        public static class Routes
        {
            public static string Home = "/";
            public static string Register = "/Register";
            public static string Logout = "/Logout";
            public static string Login = "/Login";
            public static string Chat = "/Chat";
            public static string Peticiones = "/Peticiones";
        }

        public static class StorageConstants
        {
            public static class Local
            {
                public static string Preference = "clientPreference";//TODO eliminar?


                public static string ShopCart = "shopCart";
                public static string AuthToken = "authToken";
                public static string Access_Token = "access_token";
                public static string RefreshToken = "refreshToken";
                public static string UserImageURL = "userImageURL";
                public static string ExpiryTime = "expiryTime";
                public static string IdUser = "idUser";
            }

            public static class Server
            {
                /// <summary>
                /// ESTO SOLO DEBE USARSE EN EL SEEDER
                /// </summary>
                public static string ProfilePicture = "Files\\Images\\ProfilePictures\\";
            }
        }

        public static string AdminEmail = "agusstiinbt@gmail.com";
        public static string BasicEmail = "joseespositoing@gmail.com";
        public static string DefaultPassword = "Zx2555@@";

    }
}
