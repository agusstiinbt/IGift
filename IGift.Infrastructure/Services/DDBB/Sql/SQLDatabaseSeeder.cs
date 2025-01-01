using IGift.Application.Interfaces.DDBB.Sql;
using IGift.Infrastructure.Data;
using IGift.Infrastructure.Models;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Identity;

namespace IGift.Infrastructure.Services.DDBB.Sql
{
    public class SQLDatabaseSeeder : IDatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IGiftUser> _userManager;
        private readonly RoleManager<IGiftRole> _roleManager;

        public SQLDatabaseSeeder(ApplicationDbContext context, UserManager<IGiftUser> userManager, RoleManager<IGiftRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async void Initialize()
        {
            AddAdministrator();
            AddBasicUser();
            await _context.SaveChangesAsync();
        }
        private void AddAdministrator()
        {
            Task.Run(async () =>
              {
                  var admin = AppConstants.Role.AdministratorRole;

                  var adminRole = new IGiftRole
                  {
                      Name = AppConstants.Role.AdministratorRole,
                      Description = "Rol de administrador con todos los permisos",
                      CreatedBy = admin,
                      CreatedOn = DateTime.Today,
                      LastModifiedBy = admin,
                      LastModifiedOn = DateTime.Today,
                  };

                  var adminRoleInDb = await _roleManager.FindByNameAsync(admin);
                  if (adminRoleInDb == null)
                  {
                      await _roleManager.CreateAsync(adminRole);
                      adminRoleInDb = await _roleManager.FindByNameAsync(admin);
                  }

                  var superUser = new IGiftUser
                  {
                      FirstName = "Agustin",
                      LastName = "Esposito",
                      Email = AppConstants.Server.AdminEmail,
                      UserName = "agusstiinbt",
                      EmailConfirmed = true,
                      PhoneNumberConfirmed = true,
                      CreatedOn = DateTime.Now
                  };

                  var superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);
                  if (superUserInDb == null)
                  {
                      await _userManager.CreateAsync(superUser, AppConstants.Server.DefaultPassword);
                      var UserCreatedWithId = await _userManager.FindByEmailAsync(superUser.Email);

                      UserCreatedWithId!.ProfilePictureDataUrl = "Files\\Images\\ProfilePictures" + UserCreatedWithId.Id;

                      await _userManager.AddToRoleAsync(UserCreatedWithId, admin);

                      await _context.SaveChangesAsync();

                  }

                  //TODO esto sirve para agregar los claims al usuario superUser. Para implementarlo primero debemos borrar el superUsuario de la base de datos.
                  //foreach (var permission in Permissions.GetRegisteredPermissions())
                  //{
                  //    await _roleManager.AddPermissionClaim(adminRoleInDb, permission);
                  //} }).GetAwaiter.GetResult();   

              }).GetAwaiter().GetResult();
        }
        private void AddBasicUser()
        {
            Task.Run(async () =>
            {
                var admin = AppConstants.Role.AdministratorRole;
                var basic = AppConstants.Role.BasicRole;

                var basicRole = new IGiftRole
                {
                    Name = AppConstants.Role.BasicRole,
                    Description = "Rol con permisos básicos",
                    CreatedBy = admin,
                    LastModifiedBy = admin,
                    LastModifiedOn = DateTime.Now,
                    CreatedOn = DateTime.Now
                };

                var basicRoleInDb = await _roleManager.FindByNameAsync(basic);

                if (basicRoleInDb == null)
                {
                    await _roleManager.CreateAsync(basicRole);
                }

                var basicUser = new IGiftUser
                {
                    FirstName = "Jose",
                    LastName = "Esposito",
                    Email = AppConstants.Server.BasicEmail,
                    UserName = "joseespositoing",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };

                var basicUserInDb = await _userManager.FindByEmailAsync(basicUser.Email);
                if (basicUserInDb == null)
                {
                    await _userManager.CreateAsync(basicUser, AppConstants.Server.DefaultPassword);
                    var UserCreatedWithId = await _userManager.FindByEmailAsync(basicUser.Email);
                    await _userManager.AddToRoleAsync(UserCreatedWithId, AppConstants.Role.BasicRole);

                    UserCreatedWithId.ProfilePictureDataUrl = "Files\\Images\\ProfilePictures" + UserCreatedWithId.Id;

                    await _context.SaveChangesAsync();
                }

            }).GetAwaiter().GetResult();
        }
    }
}