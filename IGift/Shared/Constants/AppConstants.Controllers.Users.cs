namespace IGift.Shared.Constants
{
    public static partial class AppConstants
    {
        public static partial class Controllers
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
        }
    }
}
