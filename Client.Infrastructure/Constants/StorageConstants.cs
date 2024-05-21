namespace Client.Infrastructure.Constants
{
    public static class StorageConstants
    {
        public static class Local
        {
            public static string Preference = "clientPreference";

            public static string AuthToken = "authToken";
            public static string RefreshToken = "refreshToken";
            public static string UserImageURL = "userImageURL";
        }

        //TODO si vamos a tener constantes que utilicemos también en el servidor entonces además de Local, debemos tener una clase Server. Entonces todo esto debería de mudarse al proyecto Shared.
    }
}
