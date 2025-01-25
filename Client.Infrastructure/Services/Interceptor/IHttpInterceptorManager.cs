using Toolbelt.Blazor;

namespace IGift.Client.Infrastructure.Services.Interceptor
{
    public interface IHttpInterceptorManager
    {
        void RegisterEvent();

        Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e);

        void DisposeEvent();
    }
}
