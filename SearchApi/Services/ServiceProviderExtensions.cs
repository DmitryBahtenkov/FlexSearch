using Core.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace SearchApi.Services
{
    public static class ServiceProviderExtensions
    {
        public static void AddObjectCreatorService(this IServiceCollection services)
        {
            services.AddTransient<ObjectCreatorFacade>();
        }
        public static void AddGetOperationsService(this IServiceCollection services)
        {
            services.AddTransient<GetOperations>();
        }

        public static void AddUpdateOperationsService(this IServiceCollection services)
        {
            services.AddTransient<UpdateOperations>();
        }
    }
    
}