using IGift.Application.Models;
using IGift.Domain.Entities;
using IGift.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace IGift.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<IGiftUser, IGiftRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IGiftRoleClaim, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        #region DBSets

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<GiftCard> GiftCards { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<LocalAdherido> LocalesAdheridos { get; set; }
        public DbSet<Peticiones> Pedidos { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<ProfilePicture> ProfilePicture { get; set; }
        public DbSet<OperacionesIntercambio> OperacionesIntercambio { get; set; }

        #endregion

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

            // Configurar la clave primaria de ProfilePicture
            builder.Entity<ProfilePicture>()
                .HasKey(p => p.Id);

            // Configurar la relación uno a uno entre IGiftUser y ProfilePicture
            builder.Entity<ProfilePicture>()
                .HasOne<IGiftUser>()
                .WithOne()
                .HasForeignKey<ProfilePicture>(p => p.IdUser)
                .IsRequired();

            // Asegurar que IdUser es único en la tabla ProfilePicture
            builder.Entity<ProfilePicture>()
                .HasIndex(p => p.IdUser)
                .IsUnique();

            //No confundir la construcción de este código con el anterior entre ProfilePicture y IGiftUser. Son distintos por la relacion que hay entre ellos:

            builder.Entity<Notification>()
           .HasOne<IGiftUser>()
           .WithMany(u => u.Notifications)
           .HasForeignKey(n => n.IdUser);

            builder.Entity<Peticiones>()
           .HasOne<IGiftUser>()
           .WithMany(u => u.Pedidos)
           .HasForeignKey(p => p.IdUser);


            #region Casos Particular
            // En este caso tenemos el caso donde clases que tienen siepre 2 o + usuarios. Y un usario tiene una coleccion de esa clase

            #region OperacionesIntercambio

            builder.Entity<OperacionesIntercambio>()
                .HasOne<IGiftUser>()
                .WithMany(x => x.OperacionesIntercambios)
                .HasForeignKey(p => p.IdUser1)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OperacionesIntercambio>()
              .HasOne<IGiftUser>()
              .WithMany(x => x.OperacionesIntercambios)
              .HasForeignKey(p => p.IdUser2)
              .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Contracts

            builder.Entity<Contract>()
               .HasOne<IGiftUser>()
               .WithMany(x => x.Contratos)
               .HasForeignKey(p => p.IdUser1)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Contract>()
              .HasOne<IGiftUser>()
              .WithMany(x => x.Contratos)
              .HasForeignKey(p => p.IdUser2)
              .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #endregion

            builder.Entity<Peticiones>(entity =>
            {
                entity.ToTable(name: "Pedidos", "dbo");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

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
