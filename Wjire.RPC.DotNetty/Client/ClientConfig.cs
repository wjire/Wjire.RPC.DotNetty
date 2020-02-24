using System;
using System.Net;
using Wjire.RPC.DotNetty.Serializer;

namespace Wjire.RPC.DotNetty.Client
{
    public class ClientConfig
    {
        public string Ip { get; set; }

        public int Port { get; set; }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(5);

        public int PooledObjectMax { get; set; } = Environment.ProcessorCount * 2;

        public int SoSndbuf { get; set; } = 64 * 1024;

        public int SoRcvbuf { get; set; } = 64 * 1024;

        internal IRpcSerializer RpcSerializer { get; set; } = new RpcJsonSerializer();

        //public int AllIdleTimeSeconds { get; set; } = 60 * 5;


    }
}
