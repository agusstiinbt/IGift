using IGift.Application.OptionsPattern;
using IGift.Shared.Constants;
using IGIFT.Server.Shared.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace IGIFT.Server.Shared
{
    internal static class ApplicationBuilderExtensions
    {

        internal static IApplicationBuilder UseExceptionHandling(
           this IApplicationBuilder app,
           IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //TODO que mas podria ir en este metodo?
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
            AppConfiguration config = GetApplicationSettings(configuration);
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
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", typeof(Program).Assembly.GetName().Name);
                options.RoutePrefix = "swagger";
                options.DisplayRequestDuration();
            });
        }

        internal static IApplicationBuilder UseEndpoints(this IApplicationBuilder app, string serviceName)
        => app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages(); // Maneja páginas Razor
            endpoints.MapControllers(); // Maneja controladores (API o MVC)
            endpoints.MapFallbackToFile("index.html"); // Sirve un archivo predeterminado si no se encuentra otro endpoint
            if (serviceName == "serviceNameDeChats")
            {
                endpoints.MapHub<SignalRHub>(AppConstants.SignalR.HubUrl); // Configura un hub de SignalR.Aplicar solo a microservicios que utilicen SignalR
            }
        });

        #region Private Methods
        private static AppConfiguration GetApplicationSettings(IConfiguration configuration)
        {
            var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
            return applicationSettingsConfiguration.Get<AppConfiguration>();
        }

        #endregion
    }
}
