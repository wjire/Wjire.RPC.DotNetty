using System;
using System.Collections.Concurrent;

namespace Wjire.RPC.DotNetty.Client
{
    public static class ClientFactory
    {
        private static readonly ConcurrentDictionary<string, Lazy<ClientGroup>> ClientGroups =
            new ConcurrentDictionary<string, Lazy<ClientGroup>>();

        public static T GetClient<T>(string ipString, int port) where T : class
        {
            return GetClient<T>(new ClientConfig(ipString, port));
        }


        public static T GetClient<T>(ClientConfig config) where T : class
        {
            ClientGroup clientGroup = GetClientGroup(config);
            return clientGroup.GetClient<T>();
        }


        private static ClientGroup GetClientGroup(ClientConfig config)
        {
            string key = $"{config.RemoteAddress}";
            Lazy<ClientGroup> group = ClientGroups.GetOrAdd(key, k =>
            {
                return new Lazy<ClientGroup>(() => new ClientGroup(config));
            });
            return group.Value;
        }
    }
}
