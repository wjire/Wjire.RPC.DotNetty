using Wjire.RPC.DotNetty.Serializer;

namespace Wjire.RPC.DotNetty
{
    public static class RpcConfig
    {
        internal static IRpcSerializer DefaultSerializer = new RpcJsonSerializer();

        public static void UseMessagePackSerializer()
        {
            DefaultSerializer = new RpcMessagePackSerializer();
        }

        public static void UserJsonSerializer()
        {
            DefaultSerializer = new RpcJsonSerializer();
        }
    }
}
