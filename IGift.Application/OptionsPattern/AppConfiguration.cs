namespace IGift.Application.OptionsPattern
{
    public class AppConfiguration
    {
        /// <summary>
        /// La propiedad Secret en la clase AppConfiguration suele utilizarse para almacenar valores confidenciales o secretos de la aplicación, como claves de autenticación, tokens de API, o secretos utilizados en la generación de JWT (JSON Web Tokens) para la autenticación y autorización. Estos valores suelen necesitarse en varias partes de la aplicación y deben mantenerse seguros. Si usas appsettings.json, puedes agregar una entrada para Secret y luego mapearla.
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// si está habilitado, indica que la aplicación está detrás de un proxy, lo cual puede requerir ajustes en las cabeceras HTTP.
        /// </summary>
        public bool BehindSSLProxy { get; set; }
        /// <summary>
        /// permite especificar una lista de IPs de proxy conocidos para mayor seguridad.
        /// </summary>
        public string ProxyIP { get; set; }
        /// <summary>
        /// configura la URL base de la aplicación, útil para CORS y otros escenarios donde se necesita conocer la URL.
        /// </summary>
        public string ApplicationUrl { get; set; }

        /// <summary>
        /// Nombre del servidor
        /// </summary>
        public string ServiceName { get; set; }
    }
}
