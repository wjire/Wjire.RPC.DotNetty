using Wjire.RPC.DotNetty.Log;
using Wjire.RPC.DotNetty.Serializer;

namespace Wjire.RPC.DotNetty
{
    internal static class RpcConfig
    {
        internal static IRpcSerializer DefaultSerializer = new RpcJsonSerializer();

        internal static ILogHandler DefaultLogHandler = new TextHandler();
    }
}
