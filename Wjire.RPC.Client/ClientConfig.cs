using System;
using Wjire.RPC.Serializer;

namespace Wjire.RPC.Client
{
    public class ClientConfig
    {
        public string Ip { get; set; }

        public int Port { get; set; }

        public int TimeOut { get; set; } = 3;

        public int PooledObjectMax { get; set; } = Environment.ProcessorCount * 2;

        public int SoSndbuf { get; set; } = 64 * 1024;

        public int SoRcvbuf { get; set; } = 64 * 1024;

        public IRpcSerializer RpcSerializer { get; set; } = new RpcJsonSerializer();

        //public int AllIdleTimeSeconds { get; set; } = 60 * 5;


    }
}
