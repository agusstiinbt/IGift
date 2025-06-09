namespace IGift.Shared.Constants
{
    public static class AppConstants
    {
        public static class Local
        {
            public static readonly string BackgroundColor = "#181A20";

            public static string ShopCart = "shopCart";
            public static string AuthToken = "authToken";
            public static string Access_Token = "access_token";
            public static string RefreshToken = "refreshToken";
            public static string UserImageURL = "userImageURL";
            public static string ExpiryTime = "expiryTime";
            public static string IdUser = "idUser";

            //TODO esto va a estar en una bbdd
            #region A reemplazar con BBDD

            public static List<string> listaDesconectado = new List<string>() { "Ofertas", "Peticiones", "Categorias", "Electrodomesticos", "Historial", "Ayuda" };

            public static List<string> listaConectado = new List<string>() { "Ofertas", "Categorias", "Chat", "Peticiones", "Electrodomesticos", "Historial", "Ayuda", "Ofertas Grandes" };

            #endregion
        }
        public static class Routes
        {
            public static string Home = "/";
            public static string Register = "/Register";
            public static string Logout = "/Logout";
            public static string Login = "/Login";
            public static string Chat = "/Chat";
            public static string Peticiones = "/Peticiones";
            public static string UserProfile = "/UserProfile";
        }
        public static class Role
        {
            public const string AdministratorRole = "Administrator";
            public const string BasicRole = "Basic";
        }
        public static class Server
        {
            /// <summary>
            /// ESTO SOLO DEBE USARSE EN EL SEEDER
            /// </summary>
            public static string ProfilePicture = "Files\\Images\\ProfilePictures\\";

            public static string AdminEmail = "agusstiinbt@gmail.com";
            public static string BasicEmail = "joseespositoing@gmail.com";
            public static string DefaultPassword = "Zx2555@@";

            /// <summary>
            /// Este servidor se conecta a SqlServer
            /// </summary>
            public const string AuthService = "AuthService";
            /// <summary>
            /// Este servidor se conecta a MongoDB
            /// </summary>
            public const string CommunicationService = "CommunicationService";


            public const string ApiGateway = "ApiGateway";
        }
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

            public const string SetLastMessageToSeen = "SetLastMessageToSeen";



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
        public static class Controller
        {
            public class Chat
            {
                private static string route = "api/Chat/";

                public static string LoadChatUsers = route + "LoadChatUsers";
                public static string GetChatById = route + "GetChatById";
                public static string SaveMessage = route + "SaveMessage";
            }
            public static partial class Files
            {
                private static string route = "api/Files/";

                public static string GetProfilePictureById = route + "GetProfilePictureById";
                public static string UploadProfilePicture = route + "UploadProfilePicture";
            }
            public static partial class Notification
            {
                private static string route = "api/Notification/";
                public static string GetAll = route + "GetAll";
                public static string SaveNotificationAsync = route + "SaveNotificationAsync";
            }
            public static partial class Peticiones
            {
                private static string route = "api/Peticiones/";
                public static string GetAll = route;
            }
            public static partial class Roles
            {
                private static string route = "api/Roles/";
            }
            public static partial class Token
            {
                private static string route = "api/Token/";
                public static string LogIn = route + "Login";
                public static string RefreshToken = route + "RefreshToken";
            }
            public static partial class Users
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
            public static partial class Titulos
            {
                private static string route = "api/Titulos/";

                public static string GetBarraHerramientasDesconectado = route + "GetBarraHerramientasDesconectado";
                public static string GetBarraHerramientasConectado = route + "GetBarraHerramientasConectado";
            }
        }
    }
}
