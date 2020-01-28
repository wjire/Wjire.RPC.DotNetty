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
            string key = $"{ipString}:{port}_{typeof(T).FullName}";
            Lazy<object> service = FuncServices.GetOrAdd(key, k =>
            {
                return new Lazy<object>(() =>
                {
                    IPEndPoint remoteAddress = new IPEndPoint(IPAddress.Parse(ipString), port);
                    Client client = new Client(remoteAddress, typeof(T), timeOut);
                    return client.ActLike<T>();
                });
            });
            return (T)service.Value;
        }

    }
}
