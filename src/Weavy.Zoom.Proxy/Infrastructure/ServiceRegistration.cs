using Microsoft.Extensions.DependencyInjection;
using Weavy.Zoom.Proxy.Data.Repos;
using Weavy.Zoom.Proxy.Services;

namespace Weavy.Zoom.Proxy.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IZoomEventRepository, ZoomEventRepository>();
            services.AddTransient<IRestService, ProxyRestService>();

        }
    }
}
