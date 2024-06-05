using IGift.Domain.Entities;
using IGift.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace IGift.Infrastructure.Data
{
    //TODO el string después de IGiftRole debería ser string o int? Fijarse en blazorHero
    public class ApplicationDbContext : IdentityDbContext<IGiftUser, IGiftRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IGiftRoleClaim, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

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

            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens", "Identity");
            });
        }
    }
}
