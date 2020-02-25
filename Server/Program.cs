using System;
using IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using Wjire.RPC.Server;

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
                          //注册服务
                          .AddSingleton<ITest, Test>()
                          .AddSingleton<IFoo, Foo>()
                          //.AddSingleton<IRpcSerializer,RpcMessagePackSerializer>()//使用 MessagePack 序列化.默认采用 Newtonsoft.Json
                          //启动服务两种方式
                          //.AddRpcServer(x=>x.Port = 7878);
                          .AddRpcServer();//读取配置文件 "ServerConfig" 节点
                  }).UseWindowsService();
        }
    }
}
