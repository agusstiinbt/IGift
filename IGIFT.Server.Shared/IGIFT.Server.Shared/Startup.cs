using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            var serviceName = _configuration.GetValue<string>("ServiceName");


            //Siempre que avancemos con estas cosas decirle a chatGPT que estamos siempre trabajando con una arquitectura de microservicios donde habrá luego una api gateway 
            services.AddForwarding(_configuration);
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
            services.AddSerialization(_configuration);
            services.AddDatabase(_configuration);


            #region A utilizar en un posible futuro

            //En el caso de que trabajemos con preferencias de lenguaje y eso:
            //services.AddScoped<ServerPreferenceManager>();

            //En el caso de que tengamos una clase especifica para traducir elementos segun el lenguaje que seleccionemos. Creo que MudBlazor ya tiene por default una herramienta parecida pero si queremos agregar lenguajes deberiamos de utilizar esto:
            //services.AddServerLocalization();


            #endregion

            if (serviceName == "AuthService")
            {
                services.AddIdentity(_configuration);
                services.AddJwtAuthentication(_configuration);
            }

            if (serviceName == "ChatService")
            {
                services.AddSignalR();
            }

            services.AddApplicationLayer();

            services.AddApplicationServices();

            services.AddRepositories();

            #region TODOs


            //TODO generar ejemplos para utilizar  services.AddExtendedAttributesUnitOfWork
            //TODO:
            //        Prueba el Swagger de cada microservicio
            //        Una vez configurado, puedes acceder a las direcciones Swagger de cada microservicio:
            //        http://localhost:5001/swagger/index.html (SQL Server)
            //        http://localhost:5002/swagger/index.html (Oracle)
            //        http://localhost:5003/swagger/index.html (MongoDB)
            //        http://localhost:5004/swagger/index.html (PostgreSQL)
            //        http://localhost:5005/swagger/index.html (MySQL)

            //TODO este metodo de registerswagger queda sin utilizarse por ahora. Mas adelante al implementar el api gateway es posible que lo podamos utilizar para integrarlo con los demas metodo de register swagger de los otros microservicios. Preguntar a ChatGPT
            //services.RegisterSwagger();

            //TODO nos quedaria agregar el AddServerStorage pero no sabemos para que sirve

            #endregion

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStringLocalizer<Startup> localizer)
        {

        }
    }
}
