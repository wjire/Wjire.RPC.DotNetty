using Microsoft.Extensions.DependencyInjection;

namespace Wjire.RPC.DotNetty
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRpcService<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            RpcServiceCollection.Singleton.AddSingleton<TService, TImplementation>();
            return services;
        }
    }
}
