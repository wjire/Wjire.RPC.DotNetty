using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Wjire.RPC.Server
{
    internal class Bootstrapper : BackgroundService
    {
        private readonly ServerConfig _serverConfig;
        private readonly IChannelHandler _channelHandler;
        private readonly ILogger _logger;
        private IEventLoopGroup _acceptor;
        private IEventLoopGroup _client;
        private IChannel _channel;
        private ServerBootstrap _bootstrap;
        private bool _isDisposed;

        public Bootstrapper(ServerConfig serverConfig, IChannelHandler channelHandler, ILogger<Bootstrapper> logger)
        {
            _serverConfig = serverConfig;
            _channelHandler = channelHandler;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await BootstrapAsync(stoppingToken);
            }
            catch (Exception)
            {
                Dispose();
                throw;
            }
        }


        public async Task BootstrapAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Rpc server is starting.");

            _acceptor = new MultithreadEventLoopGroup(_serverConfig.AcceptorEventLoopCount);
            _client = new MultithreadEventLoopGroup(_serverConfig.ClientEventLoopCount);
            // dotnetty 服务器引导程序
            _bootstrap = ServerBootstrapFactory(_serverConfig);

            _channel = await _bootstrap.BindAsync(_serverConfig.Port);

            _logger.LogInformation($"Rpc server started! The port is {_serverConfig.Port}");
        }


        private Func<ServerConfig, ServerBootstrap> ServerBootstrapFactory => serverConfig =>
         {
             return new ServerBootstrap()
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
                      pipeline.AddLast("framing-dec",
                          new LengthFieldBasedFrameDecoder(serverConfig.MaxFrameLength, 0, 4, 0, 4));
                      pipeline.AddLast(_channelHandler);
                  }));
         };


        public override void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            Task.WaitAll(_channel?.CloseAsync(), _client?.ShutdownGracefullyAsync(), _acceptor?.ShutdownGracefullyAsync());
            _isDisposed = true;
        }


        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Rpc server is closing");
            try
            {
                if (_isDisposed)
                {
                    return Task.CompletedTask;
                }
                Task.WaitAll(_channel?.CloseAsync(), _client?.ShutdownGracefullyAsync(), _acceptor?.ShutdownGracefullyAsync());
                _isDisposed = true;
                _logger.LogDebug("Rpc server is closed");
            }
            catch (Exception ex)
            {
                _logger.ClosingServerException(ex);
            }
            return Task.CompletedTask;
        }
    }
}
