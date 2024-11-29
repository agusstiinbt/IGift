using IGift.Application.Interfaces.Files;
using IGift.Application.Interfaces.Repositories;
using IGift.Application.Requests.LocalesAdheridos.Query;
using IGift.Infrastructure.Repositories;
using IGift.Infrastructure.Services.Files;
using Microsoft.Extensions.DependencyInjection;

namespace IGIFT.Server.Shared
{
    public static class SharedConfigureServices
    {
        //TODO ELIMINAR?
        public static void ConfigureServices(IServiceCollection services)
        {
            // Configuración común de servicios
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllLocalAdheridoQuery).Assembly));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddTransient<IUploadService, UploadService>();
            services.AddSwaggerGen();
        }
    }
}
