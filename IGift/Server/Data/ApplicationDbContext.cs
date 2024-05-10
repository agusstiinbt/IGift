using Duende.IdentityServer.EntityFramework.Options;
using IGift.Domain.Entities;
using IGift.Infrastructure.Models;
using IGift.Shared;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IGift.Server.Data
{
    //TODO hacer que esta clase se encuentre en Infrastructure. Dejar un readme para saber cómo hacer las migraciones para que la carpeta migrations siempre esté dentro de la aplicación Infrastructure. Dedicar tiempo a esto y hacer commits para separa cuándo funcionó y cuándo no.
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<SuperHeroe> SuperHeroes { get; set; }
        public DbSet<GiftCard> GiftCards { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<LocalAdherido> LocalesAdheridos { get; set; }
        public DbSet<InfoLocalesAdheridos> InfoLocalesAdheridos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

    }
}
