using IServices;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Wjire.RPC.DotNetty.Common;

namespace Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Wjire.RPC.DotNetty.Server.Server server = new Wjire.RPC.DotNetty.Server.Server(7878);
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITest, Test>();
            services.AddSingleton<IFoo, Foo>();
            server.RegisterServices(services);
            server.RegisterSerializer(new RMessagePackSerializer());
            server.Start().Wait();
        }
    }
}
