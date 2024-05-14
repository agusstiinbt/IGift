using System.Runtime.ConstrainedExecution;

namespace Client.Infrastructure.Routes
{
    public static class Endpoints
    {
        public class Users
        {
            private static string route = "api/Users/";


            public string GetAll = route + "GetAll";
        }

        public class Login
        {
            public static string route = "api/Login/";


            public static string LogIn = route + "Login";
        }
    }
}
