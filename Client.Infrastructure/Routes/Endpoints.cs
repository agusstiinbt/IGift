namespace Client.Infrastructure.Routes
{
    public static class Endpoints
    {
        public class Users
        {
            private static string route = "api/Users/";

            public static string GetAll = route + "GetAll";
            public static string LogIn = route + "Login";
            public static string Register = route + "Register";

        }
    }
}
