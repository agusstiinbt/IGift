﻿namespace IGift.Shared.Constants
{
    public static partial class AppConstants
    {
        public static partial class Controllers
        {
            public static class FilesController
            {
                private static string route = "api/Files/";

                public static string GetProfilePictureById = route + "GetProfilePictureById";
                public static string UploadProfilePicture = route + "UploadProfilePicture";
            }
        }
    }
}