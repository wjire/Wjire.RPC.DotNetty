using IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using Wjire.RPC.DotNetty;

namespace Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                  .ConfigureServices((hostContext, services) =>
                  {
                      services.AddRpcService<ITest, Test>();
                      //services.AddSingleton<IRpcSerializer, RpcJsonSerializer>();//默认就是 Json
                      //services.AddSingleton<IRpcSerializer, RpcMessagePackSerializer>();
                      services.AddHostedService<Wjire.RPC.DotNetty.Server>();
                  }).UseWindowsService();
        }
    }
}
