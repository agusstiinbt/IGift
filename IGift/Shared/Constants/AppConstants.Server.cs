namespace IGift.Shared.Constants
{
    public static partial class AppConstants
    {
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
            public const string ChatService = "ChatService";


            
            public const string ApiGateway = "ApiGateway";
        }
    }
}
