using IGift.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IGift.Server
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();

                    if (context.Database.IsSqlServer())
                    {
                        context.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");

                    throw;
                }
            }

            await host.RunAsync();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
         Host.CreateDefaultBuilder(args)
         .UseSerilog()
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder.UseStaticWebAssets();
                 webBuilder.UseStartup<Startup>();
             });
    }
}