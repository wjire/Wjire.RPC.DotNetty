namespace Wjire.RPC.DotNetty
{
    public static class ServerConfig
    {
        public static int SoBacklog = 1024;
        public static int SoSndbuf = 64 * 1024;
        public static int SoRcvbuf = 64 * 1024;
    }
}
