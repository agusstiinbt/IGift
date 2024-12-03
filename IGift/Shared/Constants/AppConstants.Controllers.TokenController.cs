namespace IGift.Shared.Constants
{
    public static partial class AppConstants
    {
        public static partial class Controllers
        {
            public static class TokenController
            {
                private static string route = "api/Token/";
                public static string LogIn = route + "Login";
                public static string RefreshToken = route + "RefreshToken";
            }
        }
    }
}
