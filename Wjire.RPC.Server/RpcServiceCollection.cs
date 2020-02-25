using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Wjire.RPC.Common.Message;

namespace Wjire.RPC.Server
{
    public class RpcServiceCollection
    {
        private readonly IServiceProvider _rpcServiceProvider;
        private readonly ConcurrentDictionary<string, Type> _rpcServiceFullNameAndTypeMaps;
        public RpcServiceCollection(IRpcServiceSelector rpcServiceSelector)
        {
            IServiceCollection rpcServices = rpcServiceSelector.SelectRpcServices(ServiceCollectionExtension.ServiceCollection);
            _rpcServiceFullNameAndTypeMaps = RpcServiceFullNameAndTypeMapsFactory(rpcServices);
            _rpcServiceProvider = rpcServices.BuildServiceProvider();
        }


        private Func<IServiceCollection, ConcurrentDictionary<string, Type>> RpcServiceFullNameAndTypeMapsFactory => rpcServices =>
         {
             ConcurrentDictionary<string, Type> result = new ConcurrentDictionary<string, Type>();
             foreach (ServiceDescriptor service in rpcServices)
             {
                 result.TryAdd(service.ServiceType.FullName, service.ServiceType);
             }
             return result;
         };


        internal (object, MethodInfo) FindRpcServiceAndRequestMethod(RpcRequest request)
        {
            Type rpcServiceContractType = FindRpcServiceContractType(request.ServiceContractFullName);
            object rpcService = _rpcServiceProvider.GetRequiredService(rpcServiceContractType);
            MethodInfo methodInfo = rpcServiceContractType.GetMethod(request.MethodName);
            if (methodInfo == null)
            {
                throw new InvalidOperationException($"not find {request.MethodName} method on {request.ServiceContractFullName} service");
            }
            return (rpcService, methodInfo);
        }


        private Type FindRpcServiceContractType(string rpcServiceFullName)
        {
            if (_rpcServiceFullNameAndTypeMaps.TryGetValue(rpcServiceFullName, out Type rpcServiceContractType))
            {
                return rpcServiceContractType;
            }
            throw new InvalidOperationException($"not find {rpcServiceFullName} service");
        }
    }
}
