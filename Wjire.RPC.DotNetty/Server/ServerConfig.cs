using System;

namespace Wjire.RPC.DotNetty
{
    public class ServerConfig
    {
        public int Port { get; set; }
        public int AcceptorEventLoopCount { get; set; } = 1;
        public int ClientEventLoopCount { get; set; } = Environment.ProcessorCount * 2;
        public int SoBacklog { get; set; } = 1024;
        public int SoSndbuf { get; set; } = 64 * 1024;
        public int SoRcvbuf { get; set; } = 64 * 1024;
        public int MaxFrameLength { get; set; } = int.MaxValue;

    }
}
