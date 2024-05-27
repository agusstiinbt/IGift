﻿using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Application.Requests.Identity;
using IGift.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace IGift.Client.Pages
{
    public partial class Login
    {
        [Inject] private IAuthService AuthService { get; set; }
        [Inject] private ISnackbar _snackBar { get; set; }

        private UserLoginRequest loginModel = new UserLoginRequest();

        private async Task HandleLogin()
        {
            var result = await AuthService.Login(loginModel);

            if (result.Succeeded)
            {
                NavigationManager.NavigateTo(AppConstants.Routes.Home);
            }
            else
            {
                _snackBar.Add(result.Messages.FirstOrDefault(),Severity.Error);
            }
        }
    }
}
