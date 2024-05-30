namespace IGift.Shared
{
    public static class AppConstants
    {
        public static class Role
        {
            public const string AdministratorRole = "Administrator";
            public const string BasicRole = "Basic";
            public const string DefaultPassword = "Zx2555@@";
        }

        public static class Endpoints
        {
            public static class Users
            {
                private static string route = "api/Users/";

                public static string GetAll = route + "GetAll";
                public static string Register = route + "Register";
            }

            public static class Token
            {
                private static string route = "api/Token/";
                public static string GetToken = route + "Login";
                public static string RefreshToken = route + "RefreshToken";
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
            }

            //podemos tener constantes que utilicemos también en el servidor entonces además de Local y para eso debemos tener una clase Server. 
        }
    }
}
