using System;
using System.Collections.Concurrent;
using Wjire.RPC.DotNetty.Helper;

namespace Wjire.RPC.DotNetty.Client
{
    public static class ClientFactory
    {
        private static readonly ConcurrentDictionary<string, Lazy<ClientGroup>> ClientGroups =
            new ConcurrentDictionary<string, Lazy<ClientGroup>>();


        public static T GetClient<T>() where T : class
        {
            ClientConfig clientConfig = ConfigurationHelper.GetClientConfig();
            return GetClient<T>(clientConfig);
        }


        public static T GetClient<T>(string ip, int port) where T : class
        {
            return GetClient<T>(new ClientConfig
            {
                Ip = ip,
                Port = port
            });
        }


        public static T GetClient<T>(ClientConfig config) where T : class
        {
            ClientGroup clientGroup = GetClientGroup(config);
            return clientGroup.GetClient<T>();
        }


        private static ClientGroup GetClientGroup(ClientConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Ip) || config.Port == 0)
            {
                throw new ArgumentNullException("ip or port is null");
            }
            string key = $"{config.Ip}:{config.Port}";
            Lazy<ClientGroup> group = ClientGroups.GetOrAdd(key, k =>
            {
                return new Lazy<ClientGroup>(() => new ClientGroup(config));
            });
            return group.Value;
        }
    }
}
