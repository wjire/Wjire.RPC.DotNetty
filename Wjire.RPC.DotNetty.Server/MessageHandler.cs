using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Wjire.RPC.DotNetty.Model;
using Wjire.RPC.DotNetty.Serializer;
using JsonSerializer = Wjire.RPC.DotNetty.Serializer.JsonSerializer;

namespace Wjire.RPC.DotNetty.Server
{
    public class MessageHandler
    {
        internal ISerializer Serializer = new JsonSerializer();
        private IServiceProvider _serviceProvider;
        private readonly Dictionary<string, Type> _servicesMap = new Dictionary<string, Type>();

        internal void InitServicesMap(ServiceCollection services)
        {
            foreach (ServiceDescriptor service in services)
            {
                _servicesMap.Add(service.ServiceType.FullName, service.ServiceType);
            }
            _serviceProvider = services.BuildServiceProvider();
        }


        internal byte[] GetResponseBytes(string requestString)
        {
            try
            {
                Request request = Serializer.ToObject<Request>(requestString);
                Console.Write(JsonConvert.SerializeObject(request));
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
                return Serializer.ToBytes(new Response
                {
                    Data = result,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return Serializer.ToBytes(new Response
                {
                    Message = ex.ToString(),
                });
            }
        }


        internal byte[] GetResponseBytes(Request request)
        {
            try
            {
                Console.Write(request.Arguments[0] + ",");
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
                return Serializer.ToBytes(new Response
                {
                    Data = result,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return Serializer.ToBytes(new Response
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
