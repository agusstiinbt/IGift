﻿using IGift.Application.Interfaces;
using IGift.Infrastructure.Data;
using IGift.Infrastructure.Models;
using IGift.Shared.Role;
using Microsoft.AspNetCore.Identity;

namespace IGift.Infrastructure
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IGiftUser> _userManager;
        private readonly RoleManager<IGiftRole> _roleManager;

        public DatabaseSeeder(ApplicationDbContext context, UserManager<IGiftUser> userManager, RoleManager<IGiftRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async void Initialize()
        {
            try
            {
                AddAdministrator();
                AddBasicUser();
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }

        private void AddAdministrator()
        {
            Task.Run(async () =>
              {
                  var admin = RoleConstants.AdministratorRole;

                  var adminRole = new IGiftRole(RoleConstants.AdministratorRole, "Rol de administrador con todos los permisos");
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
                      Email = "agusstiinbt@gmail.com",
                      UserName = "agusstiinbt",
                      EmailConfirmed = true,
                      PhoneNumberConfirmed = true,
                      CreatedOn = DateTime.Now
                  };

                  var superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);
                  if (superUserInDb == null)
                  {
                      await _userManager.CreateAsync(superUser, RoleConstants.DefaultPassword);
                      var UserCreatedWithId = await _userManager.FindByEmailAsync(superUser.Email);

                      await _userManager.AddToRoleAsync(UserCreatedWithId, admin);
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
                var basicRole = new IGiftRole(RoleConstants.BasicRole, "Rol con permisos básicos");
                var basicRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.BasicRole);

                if (basicRoleInDb == null)
                {
                    await _roleManager.CreateAsync(basicRole);
                }

                var basicUser = new IGiftUser
                {
                    FirstName = "Jose",
                    LastName = "Esposito",
                    Email = "joseespositoing@gmail.com",
                    UserName = "joseespositoing",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };

                var basicUserInDb = await _userManager.FindByEmailAsync(basicUser.Email);
                if (basicUserInDb == null)
                {
                    await _userManager.CreateAsync(basicUser, RoleConstants.DefaultPassword);
                    var UserCreatedWithId = await _userManager.FindByEmailAsync(basicUser.Email);
                    await _userManager.AddToRoleAsync(UserCreatedWithId, RoleConstants.BasicRole);
                }

            }).GetAwaiter().GetResult();
        }
    }
}