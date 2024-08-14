using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Client.Infrastructure.Authentication;
using Client.Infrastructure.Services.Identity.Authentication;
using Client.Infrastructure.Services.Identity.Users;
using Microsoft.AspNetCore.Components.Authorization;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Client.Infrastructure.Services.Interceptor;
using Blazored.LocalStorage;
using System.Globalization;
using Client.Infrastructure.Services.Notification;
using Client.Infrastructure.Services.Requests;
using IGift.Client.Infrastructure.Services.Files;
using IGift.Client.Infrastructure.Services.Communication.Chat;
using IGift.Client.Infrastructure.Services.CarritoDeCompras;

namespace IGift.Client.Extensions
{
    public static class WebAssemblyHostBuilderExtension
    {
        private const string ClientName = "IGift.ServerAPI";
        public static WebAssemblyHostBuilder AddRootComponents(this WebAssemblyHostBuilder builder)
        {

            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            return builder;
        }
        public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder b)
        {

            //Servicio de LocalStorage
            b.Services.AddBlazoredLocalStorage();


            //Servicios MudBlazor
            b.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 5000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
                config.SnackbarConfiguration.ClearAfterNavigation = true;
            });

            //Servicios propios
            b.Services.AddScoped<IAuthService, AuthService>()
            .AddScoped<IUserManager, UserManager>()
            .AddScoped<IGiftAuthenticationStateProvider>()
            .AddScoped<AuthenticationStateProvider, IGiftAuthenticationStateProvider>()
            .AddScoped<IHttpInterceptorManager, HttpInterceptorManager>()
            .AddScoped<INotificationService, NotificationService>()
            .AddScoped<IPeticionesService, PeticionesService>()
            .AddScoped<IProfilePicture, ProfilePictureService>()
            .AddScoped<IChatService, ChatService>()
            .AddScoped<ICarritoComprasService, CarritoComprasService>()

            .AddAuthorizationCore()

            //HTTP
            .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(ClientName).EnableIntercept(sp))

            .AddHttpClient(ClientName, client =>
            {
                client.DefaultRequestHeaders.AcceptLanguage.Clear();
                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
                client.BaseAddress = new Uri(b.HostEnvironment.BaseAddress);
            });

            b.Services.AddHttpClientInterceptor();


            return b;
        }
    }
}
