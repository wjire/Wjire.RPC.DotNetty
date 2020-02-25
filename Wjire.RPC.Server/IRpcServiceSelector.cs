using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using Microsoft.Extensions.DependencyInjection;

namespace Wjire.RPC.Server
{
    public interface IRpcServiceSelector
    {
        IServiceCollection SelectRpcServices(IServiceCollection serviceCollection);
    }


    public class DefaultRpcServiceSelector : IRpcServiceSelector
    {
        public IServiceCollection SelectRpcServices(IServiceCollection serviceCollection)
        {
            IEnumerable<ServiceDescriptor> services = ServiceCollectionExtension.ServiceCollection.Where(o =>
                o.ServiceType.IsInterface &&
                o.ServiceType.GetCustomAttribute(typeof(ServiceContractAttribute)) != null);
            ServiceCollection rpcServices = new ServiceCollection();
            ServiceProvider provider = serviceCollection.BuildServiceProvider();
            foreach (ServiceDescriptor service in services)
            {
                rpcServices.AddSingleton(service.ServiceType, provider.GetRequiredService(service.ServiceType));
            }
            return rpcServices;
        }
    }
}
