using IGift.Server.Middleware;
using Microsoft.Extensions.FileProviders;

namespace IGift.Server
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

            services.AddSerialization(_configuration);

            services.AddDatabase(_configuration);

            services.AddIdentity(_configuration);

            services.AddJwtAuthentication(services.GetApplicationSettings(_configuration));

            services.AddSignalR();

            services.AddApplicationLayer();

            services.AddApplicationServices(name);

            services.AddRepositories();

            services.RegisterSwagger();

            services.AddInfrastructureMappings();

            services.AddControllers().AddValidators();

            services.AddRazorPages();

        }

        //Si hacemos una configuracion por microServicios dentro de ConfigureServices, entonces no necesitaremos hacerlo aqui en Configure
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwarding(_configuration);

            app.UseExceptionHandling(env);

            app.UseHttpsRedirection();
            app.UseMiddleware<MyMiddleware>();

            app.UseBlazorFrameworkFiles();

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
                RequestPath = new PathString("/Files")
            });

            //Se puede utilizar para obtener la cultura segun la localizacion de tu ip
            //app.UseRequestLocalizationByCulture();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints();

            app.ConfigureSwagger();


            app.Initialize(_configuration);

        }
    }
}
