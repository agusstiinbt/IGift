using Microsoft.JSInterop;

namespace Client.Infrastructure.Extensions
{
    public static class JSExtensions
    {
        public static async ValueTask InitializeInactivityTimer<T>(this IJSRuntime js, DotNetObjectReference<T> dotNetObjectReference) where T : class
        {
            await js.InvokeAsync<object>("InitializeInactivityTimer", dotNetObjectReference);
        }
    }
}
