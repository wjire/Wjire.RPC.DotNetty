﻿using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Wjire.Log;

namespace Wjire.RPC.Server
{
    public class Server : IHostedService, IDisposable
    {
        private int _port;
        private IEventLoopGroup _acceptor;
        private IEventLoopGroup _client;
        private IChannel _channel;
        private ServerBootstrap _bootstrap;
        private bool _isClosed;
        private const string ServerConfigKeyInAppSettings = "ServerConfig";
        private const string StartupLogs = "Logs\\StartupLogs";

        public Server(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            try
            {
                Init(configuration, serviceProvider);
            }
            catch (Exception ex)
            {
                LogService.WriteText("构建服务异常:" + ex, StartupLogs);
                Task.WaitAll(_client?.ShutdownGracefullyAsync(), _acceptor?.ShutdownGracefullyAsync());
                _client = null;
                _acceptor = null;
                throw;
            }
        }

        private void Init(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            LogService.WriteText("开始初始化服务!", StartupLogs);
            ServerConfig serverConfig = configuration.GetSection(ServerConfigKeyInAppSettings).Get<ServerConfig>();
            if (serverConfig == null)
            {
                throw new ArgumentException($"未在配置文件中读取到 {ServerConfigKeyInAppSettings} 节点");
            }
            _port = serverConfig.Port;
            MessageHandler messageHandler = new MessageHandler(serviceProvider);
            ServerHandler serverHandler = new ServerHandler(messageHandler);
            _acceptor = new MultithreadEventLoopGroup(serverConfig.AcceptorEventLoopCount);
            _client = new MultithreadEventLoopGroup(serverConfig.ClientEventLoopCount);
            // 服务器引导程序
            _bootstrap = new ServerBootstrap()
                .Group(_acceptor, _client)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, serverConfig.SoBacklog)
                .Option(ChannelOption.SoSndbuf, serverConfig.SoSndbuf)
                .Option(ChannelOption.SoRcvbuf, serverConfig.SoRcvbuf)
                .Option(ChannelOption.SoReuseaddr, true)
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(4));
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(serverConfig.MaxFrameLength, 0, 4, 0, 4));
                    pipeline.AddLast(serverHandler);
                }));

            LogService.WriteText("服务初始化完成!", StartupLogs);
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                LogService.WriteText("开始启动服务!", StartupLogs);
                _channel = await _bootstrap.BindAsync(_port);
                LogService.WriteText($"服务已启动,端口号 : {_port}", StartupLogs);
                Console.WriteLine($"服务已启动,端口号 : {_port}");
            }
            catch (Exception ex)
            {
                LogService.WriteText("启动服务发生异常:" + ex, StartupLogs);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


        public void Dispose()
        {
            if (_isClosed)
            {
                return;
            }
            LogService.WriteText("开始关闭服务", StartupLogs);
            try
            {
                Task.WaitAll
                (
                    _channel?.CloseAsync(),
                _client?.ShutdownGracefullyAsync(),
                _acceptor?.ShutdownGracefullyAsync()
                );
                _isClosed = true;
                LogService.WriteText("服务已关闭!", StartupLogs);
            }
            catch (Exception ex)
            {
                LogService.WriteText("服务关闭发生异常:" + ex, StartupLogs);
            }
        }
    }
}