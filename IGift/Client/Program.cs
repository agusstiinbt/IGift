using Blazored.LocalStorage;
using Client.Infrastructure.Authentication;
using Client.Infrastructure.Services.Authentication;
using IGift.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Serilog;
using Serilog.Core;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var baseAddress = builder.Services.AddHttpClient("IGift.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


ConfigureServices(builder.Services);
/* Serilog configuration */
var levelSwitch = new LoggingLevelSwitch();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(levelSwitch)
    .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
    .WriteTo.BrowserHttp(endpointUrl: $"{baseAddress}ingest", controlLevelSwitch: levelSwitch)
    .CreateLogger();

await builder.Build().RunAsync();

void ConfigureServices(IServiceCollection services)
{
    //Servicios MudBlazor
    services.AddMudServices(config =>
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

    // Supply HttpClient instances that include access tokens when making requests to the server project
    builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("IGift.ServerAPI"));
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IGiftAuthenticationStateProvider>();
    builder.Services.AddScoped<AuthenticationStateProvider, IGiftAuthenticationStateProvider>();
    builder.Services.AddBlazoredLocalStorage();
    builder.Services.AddAuthorizationCore();
    builder.Services.AddApiAuthorization();
}
