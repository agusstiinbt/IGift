using IGift.Application.Interfaces.DDBB.Sql;
using IGift.Application.OptionsPattern;
using IGift.Server.Hubs;
using IGift.Shared.Constants;

namespace IGift.Server
{
    internal static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Middleware personalizado que configura el manejo de excepciones en una aplicación ASP.NET Core. Su propósito principal es determinar cómo deben manejarse y mostrarse las excepciones según el entorno (desarrollo, producción, etc.) Si estamos en un ambiente de desarrollo ejecutamos un middleware de net core, sino nuestros personalizados como MyMiddleware
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        internal static IApplicationBuilder UseExceptionHandling(
           this IApplicationBuilder app,
           IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            return app;
        }

        /// <summary>
        /// Habilita el uso de las configuraciones definidas en AddForwarding durante el pipeline de procesamiento de solicitudes HTTP. Es el punto en el que se aplican efectivamente las políticas de proxy y CORS en tiempo de ejecución.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        internal static IApplicationBuilder UseForwarding(this IApplicationBuilder app, IConfiguration configuration)
        {
            AppConfiguration config = ServerManager.GetApplicationSettings(configuration);
            if (config.BehindSSLProxy)
            {
                app.UseCors();
                app.UseForwardedHeaders();
            }
            return app;
        }

        internal static void ConfigureSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        internal static IApplicationBuilder UseEndpoints(this IApplicationBuilder app)
        => app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages(); // Maneja páginas Razor
            endpoints.MapControllers(); // Maneja controladores (API o MVC)
            endpoints.MapFallbackToFile("index.html"); // Sirve un archivo predeterminado si no se encuentra otro endpoint
            endpoints.MapHub<SignalRHub>(AppConstants.SignalR.HubUrl); // Configura un hub de SignalR.Aplicar solo a microservicios que utilicen SignalR
        });

        /// <summary>
        /// Este metodo tiene como propósito ejecutar tareas de inicialización al inicio de la aplicación, como poblar la base de datos con datos iniciales (usuarios administradores, configuraciones básicas, etc.). 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="_configuration"></param>
        /// <returns></returns>
        internal static IApplicationBuilder Initialize(this IApplicationBuilder app, IConfiguration _configuration)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var initializers = serviceScope.ServiceProvider.GetServices<IDatabaseSeeder>();

            foreach (var initializer in initializers)
            {
                initializer.Initialize();
            }

            return app;
        }
    }
}
