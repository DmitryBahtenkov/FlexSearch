using Core.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace SearchApi.Services
{
    public static class ServiceProviderExtensions
    {
        public static void AddAllServices(this IServiceCollection services)
        {
            services.AddTransient<SearcherService>();
            services.AddTransient<UpdateOperations>();
            services.AddTransient<GetOperations>();
            services.AddTransient<ObjectCreatorFacade>();
            services.AddTransient<UserService>();
                //services.AddSingleton<DatabaseService>();
        }
    }
    
}