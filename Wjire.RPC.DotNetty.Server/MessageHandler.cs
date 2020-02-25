using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Wjire.Log;
using Wjire.RPC.DotNetty.Model;
using Wjire.RPC.DotNetty.Serializer;

namespace Wjire.RPC.Server
{
    internal class MessageHandler
    {
        private readonly IRpcSerializer _serializer;
        private readonly RpcServiceCollection _rpcServiceCollection;

        internal MessageHandler(IServiceProvider serviceProvider)
        {
            _serializer = serviceProvider.GetService<IRpcSerializer>() ?? new RpcJsonSerializer();
            _rpcServiceCollection = new RpcServiceCollection(serviceProvider);
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
            (object rpcService, MethodInfo methodInfo) = _rpcServiceCollection.FindRpcServiceAndRequestMethod(request);
            CheckArguments(request.Arguments, methodInfo.GetParameters());
            object result = methodInfo.Invoke(rpcService, request.Arguments);
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
