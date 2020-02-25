using System;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Wjire.RPC.Serializer;

namespace Wjire.RPC.Server
{
    public static class ServiceCollectionExtension
    {
        internal static IServiceCollection ServiceCollection;
        private const string ServerConfigKeyInAppSettings = "ServerConfig";

        public static IServiceCollection AddRpcServer(this IServiceCollection services, Action<ServerConfig> options = null)
        {
            ServiceCollection = services;
            services.TryAddSingleton<RpcServiceCollection>();
            services.TryAddSingleton<MessageHandler>();
            services.TryAddSingleton<IChannelHandler, ServerHandler>();
            services.TryAddSingleton<IRpcSerializer, RpcJsonSerializer>();
            services.TryAddSingleton<IRpcServiceSelector, DefaultRpcServiceSelector>();

            ServerConfig serverConfig = CreateServerConfig(options);
            services.TryAddSingleton(serverConfig);
            services.Configure<HostOptions>(option =>
            {
                option.ShutdownTimeout = TimeSpan.FromSeconds(30);
            });
            services.AddHostedService<Bootstrapper>();
            return services;
        }

        private static ServerConfig CreateServerConfig(Action<ServerConfig> options)
        {
            if (options == null)
            {
                return ServiceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>().GetSection(ServerConfigKeyInAppSettings).Get<ServerConfig>();
            }

            ServerConfig serverConfig = new ServerConfig();
            options(serverConfig);
            return serverConfig;
        }
    }
}
