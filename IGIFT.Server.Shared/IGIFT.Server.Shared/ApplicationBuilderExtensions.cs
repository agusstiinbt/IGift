using IGift.Application.AppConfiguration;
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


        #region Private Methods
        private static AppConfiguration GetApplicationSettings(IConfiguration configuration)
        {
            var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
            return applicationSettingsConfiguration.Get<AppConfiguration>();
        }

        #endregion
    }
}
