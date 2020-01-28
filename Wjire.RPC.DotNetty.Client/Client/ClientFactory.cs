using System;
using System.Collections.Concurrent;
using ImpromptuInterface;

namespace Wjire.RPC.DotNetty.Client
{
    public static class ClientFactory
    {
        private static readonly ConcurrentDictionary<string, Lazy<object>> FuncServices = new ConcurrentDictionary<string, Lazy<object>>();


        public static T GetClient<T>(string ipString, int port) where T : class
        {
            return GetClient<T>(new ClientConfig(ipString, port));
        }


        public static T GetClient<T>(ClientConfig config) where T : class
        {
            string key = $"{config.RemoteAddress}_{typeof(T).FullName}";
            Lazy<object> service = FuncServices.GetOrAdd(key, k =>
            {
                return new Lazy<object>(() =>
                {
                    Client client = new Client(typeof(T), config);
                    return client.ActLike<T>();
                });
            });
            return (T)service.Value;
        }
    }
}
