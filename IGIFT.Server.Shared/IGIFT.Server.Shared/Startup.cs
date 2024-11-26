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
            //Siempre que avancemos con estas cosas decirle a chatGPT que estamos siempre trabajando con una arquitectura de microservicios donde habrá luego una api gateway 
            services.AddForwarding(_configuration);
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
            services.AddSerialization(_configuration);
            services.AddDatabase(_configuration);

            //TODO antes de continuar con otros services fijarse bien para que sirve este y como funciona bien todo lo que es unit of work y el dispose. Asi mismo fijarse lo que serian los otros repositorios que BlazorHero en su service como Document o brand
            services.AddRepositories();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStringLocalizer<Startup> localizer)
        {

        }
    }
}
