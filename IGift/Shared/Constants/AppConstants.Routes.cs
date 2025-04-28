namespace IGift.Shared.Constants
{
    public static partial class AppConstants
    {
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
    }

    public class GetChatById
    {
        public string IdUser { get; set; } = string.Empty;
    }
}
