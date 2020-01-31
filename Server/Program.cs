using IServices;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Wjire.RPC.DotNetty;

namespace Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            RpcLogService.UseConsoleLog();
            //.net core 自带的DI容器
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITest, Test>();

            Wjire.RPC.DotNetty.Server server = new Wjire.RPC.DotNetty.Server(7878);
            server.RegisterServices(services);
            server.Start().Wait();
        }
    }
}
