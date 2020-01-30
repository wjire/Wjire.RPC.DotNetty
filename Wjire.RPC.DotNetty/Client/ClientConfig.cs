using System;
using System.Net;

namespace Wjire.RPC.DotNetty.Client
{
    public class ClientConfig
    {
        internal IPEndPoint RemoteAddress { get; }
        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(5);
        public int AllIdleTimeSeconds { get; set; } = 60 * 5;
        public int PooledObjectMax { get; set; } = Environment.ProcessorCount * 2;

        public ClientConfig(string ipString, int port)
        {
            RemoteAddress = new IPEndPoint(IPAddress.Parse(ipString), port);
        }
    }
}
