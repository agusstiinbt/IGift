using IGift.Domain.Entities;
using IGift.Infrastructure.Models;
using IGift.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace IGift.Infrastructure.Data
{
    //TODO hacer que esta clase se encuentre en Infrastructure. Dejar un readme para saber cómo hacer las migraciones para que la carpeta migrations siempre esté dentro de la aplicación Infrastructure. Dedicar tiempo a esto y hacer commits para separa cuándo funcionó y cuándo no.
    public class ApplicationDbContext : IdentityDbContext<IGiftUser, IGiftRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IGiftRoleClaim, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<SuperHeroe> SuperHeroes { get; set; }
        public DbSet<GiftCard> GiftCards { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<LocalAdherido> LocalesAdheridos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.Name is "LastModifiedBy" or "CreatedBy"))
            {
                property.SetColumnType("nvarchar(128)");
            }

            //
            base.OnModelCreating(builder);

            builder.Entity<IGiftUser>(entity =>
            {
                entity.ToTable(name: "Users", "Identity");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            builder.Entity<IGiftRole>(entity =>
            {
                entity.ToTable(name: "Roles", "Identity");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims", "Identity");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins", "Identity");
            });

            builder.Entity<IGiftRoleClaim>(entity =>
            {
                entity.ToTable(name: "RoleClaims", "Identity");

                //entity.HasOne(d => d.Role)
                //   .WithMany(p => p.RoleClaims)
                //   .HasForeignKey(d => d.RoleId);
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens", "Identity");
            });
        }
    }
}
