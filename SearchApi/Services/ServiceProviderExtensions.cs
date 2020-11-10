using Microsoft.Extensions.DependencyInjection;

namespace SearchApi.Services
{
    public static class ServiceProviderExtensions
    {
        public static void AddObjectCreatorService(this IServiceCollection services)
        {
            services.AddTransient<ObjectCreatorFacade>();
        }
    }
}