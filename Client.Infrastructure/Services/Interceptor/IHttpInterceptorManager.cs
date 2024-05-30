using Toolbelt.Blazor;

namespace Client.Infrastructure.Services.Interceptor
{
    public interface IHttpInterceptorManager
    {
        void RegisterEvent();

        Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e);

        void DisposeEvent();
    }
}
