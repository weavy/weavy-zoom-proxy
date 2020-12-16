using Microsoft.Extensions.DependencyInjection;
using Weavy.Zoom.Proxy.Data.Repos;

namespace Weavy.Zoom.Proxy.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IZoomEventRepository, ZoomEventRepository>();
            
        }
    }
}
