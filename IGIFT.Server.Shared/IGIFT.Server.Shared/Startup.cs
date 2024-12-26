using IGift.Shared.Constants;
using IGIFT.Server.Shared.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;

namespace IGIFT.Server.Shared
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            var name = _configuration.GetValue<string>("ServiceName")!;

            services.AddForwarding(_configuration);

            #region Preferecias de lenguaje 
            //configura la localización en la aplicación. Este método es parte de las funcionalidades de .NET Core para soportar aplicaciones multilingües o que manejan textos específicos según la cultura o idioma del usuario.
            ////services.AddLocalization(options =>
            ////{
            ////    options.ResourcesPath = "Resources";
            ////});
            #endregion

            services.AddSerialization(_configuration);

            if (name != AppConstants.Server.ApiGateway)
            {
                services.AddDatabase(_configuration);
            }

            #region A utilizar en un posible futuro para los lenguajes

            //En el caso de que trabajemos con preferencias de lenguaje y eso:
            //services.AddScoped<ServerPreferenceManager>();

            //En el caso de que tengamos una clase especifica para traducir elementos segun el lenguaje que seleccionemos. Creo que MudBlazor ya tiene por default una herramienta parecida pero si queremos agregar lenguajes deberiamos de utilizar esto:
            //services.AddServerLocalization();


            #endregion

            if (name == AppConstants.Server.ApiGateway)
            {
                services.AddJwtAuthentication(_configuration);
                services.AddCentralizeSwagger();
            }

            if (name == AppConstants.Server.AuthService)
            {
                services.AddIdentity(_configuration);
            }

            if (name == AppConstants.Server.CommunicationService)
            {
                services.AddSharedInfraestructure(_configuration);
                services.AddSignalR();
            }

            #region Servicios comunes para todas las apis
            services.AddApplicationLayer();


            #endregion


            services.AddApplicationServices(name);

            services.AddRepositories();

            //TODO fijarse si deberia de estar dentro de algun en particular o suelto
            services.AddInfrastructureMappings();

            services.AddRazorPages();


            #region TODOs
            //TODO fijarse el metodo addcontrollers y addvalidators. Deberia el addcontrollers estar en algun lugar particular o suelto? Y fijarse como funcionaria el addvalidators porque dependeria de quein estaria usando el controllers para que tome el assembly si es usado
            //TODO  addhangifire se usa para guardar tareas de segundo plano y se deberia usar redis y guarda la info de la microservice api gateway.  y hangfireserver?
            //TODO generar ejemplos para utilizar  services.AddExtendedAttributesUnitOfWork

            //TODO services.adlazycache fijarse quien debria de usarlo en el caso deque lo usemos tambien con redis

            //TODO nos quedaria agregar el AddServerStorage pero no sabemos para que nos serviria

            #endregion
        }

        //Si hacemos una configuracion por microServicios dentro de ConfigureServices, entonces no necesitaremos hacerlo aqui en Configure
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStringLocalizer<Startup> localizer)
        {
            var serviceName = _configuration.GetValue<string>("ServiceName")!;

            app.UseForwarding(_configuration);

            app.UseExceptionHandling(env);

            app.UseHttpsRedirection();

            app.UseMiddleware<MyMiddleware>();

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
                RequestPath = new PathString("/Files")
            });

            //Se puede utilizar para obtener la cultura segun la localizacion de tu ip
            //app.UseRequestLocalizationByCulture();

            app.UseRouting();

            if (serviceName == AppConstants.Server.AuthService)
            {
                app.UseAuthentication();
                app.UseAuthorization();
            }

            app.UseEndpoints(serviceName);

            app.ConfigureSwagger();
        }
    }
}
