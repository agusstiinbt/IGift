using System.Globalization;
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
        var host = builder.Build();
        await builder.Build().RunAsync();
    }
}