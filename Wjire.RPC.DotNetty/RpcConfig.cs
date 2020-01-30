using Wjire.RPC.DotNetty.Serializer;

namespace Wjire.RPC.DotNetty
{
    internal static class RpcConfig
    {
        internal static ISerializer DefaultSerializer = new RJsonSerializer();
    }
}
