using System;
using System.Collections.Concurrent;
using System.Net;
using ImpromptuInterface;

namespace Wjire.RPC.DotNetty.Client
{
    public static class ClientFactory
    {
        private static readonly ConcurrentDictionary<string, Lazy<object>> FuncServices = new ConcurrentDictionary<string, Lazy<object>>();


        public static T GetClient<T>(string ipString, int port) where T : class
        {
            return GetClient<T>(ipString, port, TimeSpan.FromSeconds(30));
        }


        public static T GetClient<T>(string ipString, int port, TimeSpan timeOut) where T : class
        {
            var key = $"{ipString}:{port}_{typeof(T).FullName}";
            Lazy<object> service = FuncServices.GetOrAdd(key, k =>
            {
                return new Lazy<object>(() =>
                {
                    Client client = new Client
                    {
                        IPEndPoint = new IPEndPoint(IPAddress.Parse(ipString), port),
                        ServiceType = typeof(T),
                        TimeOut = timeOut,
                    };
                    return client.ActLike<T>();
                });
            });
            return (T)service.Value;
        }

    }
}
