using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Wjire.Log;
using Wjire.RPC.DotNetty.Serializer;

namespace Wjire.RPC.DotNetty
{
    public class Server : IHostedService, IDisposable
    {
        private int _port;
        private IEventLoopGroup _acceptor;
        private IEventLoopGroup _client;
        private IChannel _channel;
        private ServerBootstrap _bootstrap;
        private bool _isClosed;
        private const string ServerConfig = "ServerConfig";

        public Server(IConfiguration configuration) : this(configuration, new RpcJsonSerializer()) { }


        public Server(IConfiguration configuration, IRpcSerializer rpcSerializer)
        {
            try
            {
                Init(configuration, rpcSerializer);
            }
            catch (Exception ex)
            {
                LogService.WriteText("构建服务异常:" + ex);
                Task.WaitAll(_client?.ShutdownGracefullyAsync(), _acceptor?.ShutdownGracefullyAsync());
                _client = null;
                _acceptor = null;
                throw;
            }
        }

        private void Init(IConfiguration configuration, IRpcSerializer rpcSerializer)
        {
            LogService.WriteText("开始初始化服务!");

            ServerConfig serverConfig = configuration.GetSection(ServerConfig).Get<ServerConfig>();
            _port = serverConfig.Port;
            ServerInvoker invoker = new ServerInvoker(rpcSerializer, RpcServiceCollection.Singleton);
            ServerHandler handler = new ServerHandler(invoker);
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
                    pipeline.AddLast(handler);
                }));

            LogService.WriteText("服务初始化完成!");
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                LogService.WriteText("开始启动服务!");
                _channel = await _bootstrap.BindAsync(_port);
                LogService.WriteText($"服务已启动,端口号 : {_port}");
            }
            catch (Exception ex)
            {
                LogService.WriteText("启动服务发生异常:" + ex);
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
            LogService.WriteText("开始关闭服务");
            try
            {
                Task.WaitAll
                (
                    _channel?.CloseAsync(),
                _client?.ShutdownGracefullyAsync(),
                _acceptor?.ShutdownGracefullyAsync()
                );
                _isClosed = true;
                LogService.WriteText("服务已关闭!");
            }
            catch (Exception ex)
            {
                LogService.WriteText("服务关闭发生异常:" + ex);
            }
        }
    }
}
