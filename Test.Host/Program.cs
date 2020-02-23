﻿using System;
using IServices;
using Microsoft.Extensions.Hosting;
using Services;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Host
{
    class Program
    {
        private static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton<ITest, Services.Test>()
                        .AddSingleton<IFoo, Foo>()
                        //.AddSingleton<IRpcSerializer, RpcJsonSerializer>()//默认就是 Json
                        //.AddSingleton<IRpcSerializer, RpcMessagePackSerializer>();
                        .AddHostedService<Wjire.RPC.DotNetty.Server>();
                }).UseWindowsService();
        }
    }
}