using System;
using System.Net;
using Wjire.RPC.DotNetty.Serializer;

namespace Wjire.RPC.DotNetty.Client
{
    public class ClientConfig
    {
        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(5);
        //public int AllIdleTimeSeconds { get; set; } = 60 * 5;
        public int PooledObjectMax { get; set; } = Environment.ProcessorCount * 2;
        public int SoSndbuf { get; set; } = 64 * 1024;
        public int SoRcvbuf { get; set; } = 64 * 1024;
        public IRpcSerializer RpcSerializer { get; set; } = new RpcJsonSerializer();
        internal IPEndPoint RemoteAddress { get; }

        public ClientConfig(string ipString, int port)
        {
            RemoteAddress = new IPEndPoint(IPAddress.Parse(ipString), port);
        }
    }
}
