using System;
using System.Net;

namespace Wjire.RPC.DotNetty
{
    public static class ServerConfig
    {
        public const int SoBacklog = 1024;
        public const int SoSndbuf = 64 * 1024;
        public const int SoRcvbuf = 64 * 1024;
    }
}
