namespace IGift.Shared
{
    public static class AppConstants
    {
        public static class SignalR
        {
            public const string ReceiveMessage = "ReceiveMessage";
            public const string ReceiveChatNotification = "ReceiveChatNotification";
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
        }

        public static class Routes
        {
            public static string Home = "/";
            public static string Register = "/Register";
            public static string Logout = "/Logout";
            public static string Login = "/Login";
        }

        public static class StorageConstants
        {
            public static class Local
            {
                public static string Preference = "clientPreference";

                public static string AuthToken = "authToken";
                public static string RefreshToken = "refreshToken";
                public static string UserImageURL = "userImageURL";
                public static string ExpiryTime = "expiryTime";
                public static string IdUser = "idUser";
            }

            //podemos tener constantes que utilicemos también en el servidor entonces además de Local y para eso debemos tener una clase Server. 
        }

        public static string AdminEmail = "agusstiinbt@gmail.com";
        public static string BasicEmail = "joseespositoing@gmail.com";
        public static string DefaultPassword = "Zx2555@@";

    }
}
