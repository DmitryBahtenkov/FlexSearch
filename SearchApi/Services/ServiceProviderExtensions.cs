using Core.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace SearchApi.Services
{
    public static class ServiceProviderExtensions
    {
        public static void AddAllServices(this IServiceCollection services)
        {
            services.AddTransient<SearcherService>();
            services.AddTransient<UserService>();
                //services.AddSingleton<DatabaseService>();
        }
    }
    
}