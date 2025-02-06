using Toolbelt.Blazor;

namespace IGift.Client.Infrastructure.Services.Interceptor
{
    public interface IHttpInterceptorManager
    {
        /// <summary>
        /// Esto solo debe estar en vistas generales como MainLayout NUNCA debe estar en paginas individuales como index o productos
        /// </summary>
        void RegisterEvent();

        Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e);

        void DisposeEvent();
    }
}
