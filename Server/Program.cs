using IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;

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
                      services
                          .AddSingleton<ITest, Test>()
                          .AddSingleton<IFoo, Foo>()
                        //.AddSingleton<IRpcSerializer, RpcJsonSerializer>()//默认就是 Json
                        //.AddSingleton<IRpcSerializer, RpcMessagePackSerializer>();
                          .AddHostedService<Wjire.RPC.DotNetty.Server>();
                  }).UseWindowsService();
        }
    }
}
