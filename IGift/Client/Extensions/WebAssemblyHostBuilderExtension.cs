using System.Globalization;
using Blazored.LocalStorage;
using Client.Infrastructure.Authentication;
using Client.Infrastructure.Services.Identity.Authentication;
using Client.Infrastructure.Services.Identity.Users;
using Client.Infrastructure.Services.Interceptor;
using Client.Infrastructure.Services.Notification;
using IGift.Client.Infrastructure.Authentication;
using IGift.Client.Infrastructure.Services.CarritoDeCompras;
using IGift.Client.Infrastructure.Services.Communication.Chat;
using IGift.Client.Infrastructure.Services.Files;
using IGift.Client.Infrastructure.Services.Identity.Authentication;
using IGift.Client.Infrastructure.Services.Interceptor;
using IGift.Client.Infrastructure.Services.Peticiones;
using IGift.Client.Infrastructure.Services.Titulos.Categoria;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;

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
            b.Services


           .AddAuthorizationCore()

            //Servicio de LocalStorage
          .AddBlazoredLocalStorage()

            //Servicios MudBlazor
          .AddMudServices(config =>
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
            })

          .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())

            //Servicios propios

            .AddScoped<IGiftAuthenticationStateProvider>()
            .AddScoped<AuthenticationStateProvider, IGiftAuthenticationStateProvider>()


            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IUserManager, UserManager>()
            .AddScoped<IHttpInterceptorManager, HttpInterceptorManager>()
            .AddScoped<INotificationService, NotificationService>()
            .AddScoped<IPeticionesService, PeticionesService>()
            .AddScoped<IProfilePicture, ProfilePictureService>()
            .AddScoped<IChatManager, ChatManager>()
            .AddScoped<IShopCart, ShopCartService>()
            .AddScoped<ITitulosService, TitulosService>()

             //HTTP
             .AddTransient<AuthenticationHeaderHandler>()
                .AddScoped(sp => sp
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(ClientName).EnableIntercept(sp))
                .AddHttpClient(ClientName, client =>
                {
                    client.DefaultRequestHeaders.AcceptLanguage.Clear();
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
                    client.BaseAddress = new Uri(b.HostEnvironment.BaseAddress);
                })
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();

            b.Services.AddHttpClientInterceptor();

            return b;
        }
    }
}
