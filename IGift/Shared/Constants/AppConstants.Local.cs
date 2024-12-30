namespace IGift.Shared.Constants
{
    public static partial class AppConstants
    {
        public static class Local
        {
            public static readonly string BackgroundColor = "#181A20";

            public static string ShopCart = "shopCart";
            public static string AuthToken = "authToken";
            public static string Access_Token = "access_token";
            public static string RefreshToken = "refreshToken";
            public static string UserImageURL = "userImageURL";
            public static string ExpiryTime = "expiryTime";
            public static string IdUser = "idUser";

            //TODO esto va a estar en una bbdd
            #region A reemplazar con BBDD

            public static List<string> listaDesconectado = new List<string>() { "Ofertas", "Peticiones", "Categorias", "Electrodomesticos", "Historial", "Ayuda" };

            public static List<string> listaConectado = new List<string>() { "Ofertas", "Categorias", "Chat", "Peticiones", "Electrodomesticos", "Historial", "Ayuda", "Ofertas Grandes" };

            public static List<string> listaCategorias = new List<string>() { "Vehiculos", "Inmuebles", "Supermercado", "Tecnologia", "Compra internacional", "Hogar y Muebles", "Electrodomesticos", "Servicios", "Salud y Belleza" };
            #endregion
        }
    }
}
