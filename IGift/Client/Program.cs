using IGift.Client.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

//var builder = WebAssemblyHostBuilder.CreateDefault(args).AddRootComponents().AddClientServices();

//await builder.Build().RunAsync();

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder
                      .CreateDefault(args)
                      .AddRootComponents()
                      .AddClientServices();

        builder.Logging.SetMinimumLevel(LogLevel.Debug);

        var host = builder.Build();
        await host.RunAsync();
    }
}
