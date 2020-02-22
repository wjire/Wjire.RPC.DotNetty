using Microsoft.Extensions.DependencyInjection;

namespace Wjire.RPC.DotNetty
{
    public class RpcServiceCollection : ServiceCollection
    {
        public static RpcServiceCollection Singleton = new RpcServiceCollection();
        private RpcServiceCollection() { }
    }
}
