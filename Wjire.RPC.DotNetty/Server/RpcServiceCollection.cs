using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using Wjire.RPC.DotNetty.Model;

namespace Wjire.RPC.DotNetty
{
    internal class RpcServiceCollection
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, Type> _rpcServiceFullNameAndTypeMaps = new ConcurrentDictionary<string, Type>();

        internal RpcServiceCollection(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            List<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.IsInterface && t.GetCustomAttribute(typeof(ServiceContractAttribute)) != null)).ToList();
            if (types.Count == 0)
            {
                throw new Exception("未注册任何 RpcService,请确认您需要注册的服务契约上包含 [ServiceContract] 特性");
            }

            foreach (Type type in types)
            {
                _rpcServiceFullNameAndTypeMaps.TryAdd(type.FullName, type);
            }
        }


        internal (object, MethodInfo) FindRpcServiceAndRequestMethod(RpcRequest request)
        {
            Type rpcServiceContractType = FindRpcServiceContractType(request.ServiceContractFullName);
            object rpcService = _serviceProvider.GetService(rpcServiceContractType);
            MethodInfo methodInfo = rpcService.GetType().GetMethod(request.MethodName);
            if (methodInfo == null)
            {
                throw new ArgumentException($"not find the method:{request.MethodName} on service:{request.ServiceContractFullName}");
            }
            return (rpcService, methodInfo);
        }


        internal Type FindRpcServiceContractType(string rpcServiceFullName)
        {
            if (_rpcServiceFullNameAndTypeMaps.TryGetValue(rpcServiceFullName, out Type rpcServiceContractType))
            {
                return rpcServiceContractType;
            }
            throw new ArgumentNullException($"未找到名称为 {rpcServiceFullName} 的服务");
        }
    }
}
