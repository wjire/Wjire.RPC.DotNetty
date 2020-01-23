using System;
using IServices;
using Microsoft.Extensions.DependencyInjection;
using Services;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Wjire.RPC.DotNetty.Server.Server(7878);
            var services = new ServiceCollection();
            services.AddSingleton<ITest, Test>();
            services.AddSingleton<IFoo, Foo>();
            server.RegisterServices(services);
            server.Start().Wait();
        }
    }
}
