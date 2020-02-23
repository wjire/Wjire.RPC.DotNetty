using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Wjire.Log;
using Wjire.RPC.DotNetty.Model;
using Wjire.RPC.DotNetty.Serializer;

namespace Wjire.RPC.DotNetty
{
    internal class ServerInvoker
    {
        private readonly IRpcSerializer _serializer;
        private readonly IServiceProvider _serviceProvider;

        internal ServerInvoker(IServiceProvider serviceProvider)
        {
            _serializer = serviceProvider.GetService<IRpcSerializer>() ?? new RpcJsonSerializer();
            _serviceProvider = serviceProvider;
        }
        
        internal byte[] GetResponseBytes(byte[] requestBytes)
        {
            RpcRequest request = null;
            try
            {
                request = _serializer.ToObject<RpcRequest>(requestBytes);
                return GetResponseBytes(request);
            }
            catch (Exception ex)
            {
                LogService.WriteExceptionAsync(ex, "GetResponseBytes", request);
                return _serializer.ToBytes(new RpcResponse
                {
                    Message = ex.ToString(),
                });
            }
        }


        private byte[] GetResponseBytes(RpcRequest request)
        {
            object service = _serviceProvider.GetService(request.ServiceType);
            MethodInfo methodInfo = service.GetType().GetMethod(request.MethodName);
            if (methodInfo == null)
            {
                throw new ArgumentException($"not find the method:{request.MethodName} on service:{request.ServiceType.FullName}");
            }
            CheckArguments(request.Arguments, methodInfo.GetParameters());
            object result = methodInfo.Invoke(service, request.Arguments);
            return _serializer.ToBytes(new RpcResponse
            {
                Data = result,
                Success = true
            });
        }
        

        private void CheckArguments(object[] arguments, ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                Type argumentType = arguments[i].GetType();
                if (argumentType == parameters[i].ParameterType)
                {
                    continue;
                }

                if (argumentType.IsAssignableFrom(typeof(IConvertible)))
                {
                    arguments[i] = Convert.ChangeType(arguments[i], parameters[i].ParameterType);
                }
                else
                {
                    arguments[i] = _serializer.ToObject(arguments[i], parameters[i].ParameterType);
                }
            }
        }
    }
}
