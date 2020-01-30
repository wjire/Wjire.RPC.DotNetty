using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Wjire.RPC.DotNetty.Model;
using Wjire.RPC.DotNetty.Serializer;

namespace Wjire.RPC.DotNetty.Server
{
    internal class ServerInvoker
    {
        private readonly ISerializer _serializer;
        private IServiceProvider _serviceProvider;
        private readonly Dictionary<string, Type> _servicesMap = new Dictionary<string, Type>();

        internal ServerInvoker() : this(RpcConfig.DefaultSerializer) { }

        internal ServerInvoker(ISerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }


        internal void InitServicesMap(ServiceCollection services)
        {
            foreach (ServiceDescriptor service in services)
            {
                _servicesMap.Add(service.ServiceType.FullName, service.ServiceType);
            }
            _serviceProvider = services.BuildServiceProvider();
        }


        internal byte[] GetResponseBytes(byte[] requestBytes)
        {
            try
            {
                RpcRequest request = _serializer.ToObject<RpcRequest>(requestBytes);
                if (_servicesMap.TryGetValue(request.ServiceName, out Type serviceType) == false)
                {
                    throw new ArgumentException($"not find the service : {request.ServiceName}");
                }

                MethodInfo methodInfo = serviceType.GetMethod(request.MethodName);
                if (methodInfo == null)
                {
                    throw new ArgumentException($"not find the method:{request.MethodName} on service:{request.ServiceName}");
                }
                CheckArguments(request.Arguments, methodInfo.GetParameters());
                object service = _serviceProvider.GetService(serviceType);
                object result = methodInfo.Invoke(service, request.Arguments);
                return _serializer.ToBytes(new RpcResponse
                {
                    Data = result,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return _serializer.ToBytes(new RpcResponse
                {
                    Message = ex.ToString(),
                });
            }
        }

        private void CheckArguments(object[] arguments, ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (arguments[i].GetType() != parameters[i].ParameterType)
                {
                    arguments[i] = Convert.ChangeType(arguments[i], parameters[i].ParameterType);
                }
            }
        }
    }
}
