using System;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Wjire.Log;

namespace Wjire.RPC.DotNetty
{
    public class Server
    {
        private readonly int _port;
        private readonly IEventLoopGroup _acceptor;
        private readonly IEventLoopGroup _client;
        private readonly ServerBootstrap _bootstrap;
        private readonly ServerInvoker _messageHandler = new ServerInvoker();

        public Server(int port)
        {
            try
            {
                _port = port;
                LogService.WriteText($"{DateTime.Now} 开始构建服务!");
                ServerHandler handler = new ServerHandler(_messageHandler);
                _acceptor = new MultithreadEventLoopGroup(1);
                _client = new MultithreadEventLoopGroup();
                // 服务器引导程序
                _bootstrap = new ServerBootstrap()
                    .Group(_acceptor, _client)
                    .Channel<TcpServerSocketChannel>()
                    .Option(ChannelOption.SoBacklog, ServerConfig.SoBacklog)
                    .Option(ChannelOption.SoSndbuf, ServerConfig.SoSndbuf)
                    .Option(ChannelOption.SoRcvbuf, ServerConfig.SoRcvbuf)
                    .Option(ChannelOption.SoReuseaddr, true)
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        pipeline.AddLast("framing-enc", new LengthFieldPrepender(8));
                        pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 8, 0, 8));
                        pipeline.AddLast(handler);
                    }));
                LogService.WriteText($"{DateTime.Now} 服务构建完成!");
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex, "构建服务异常");
                //执行很慢.
                Task.WaitAll(_client?.ShutdownGracefullyAsync(), _acceptor?.ShutdownGracefullyAsync());
                _client = null;
                _acceptor = null;
                throw;
            }
        }

        public Server RegisterServices(ServiceCollection services)
        {
            _messageHandler.InitServicesMap(services);
            return this;
        }


        public async Task Start()
        {
            IChannel channel = null;
            try
            {
                LogService.WriteText($"{DateTime.Now} 开始启动服务!");
                channel = await _bootstrap.BindAsync(_port);
                LogService.WriteText($"{DateTime.Now} 服务已启动,端口号 : {_port},按 'Q' 键退出");
                do
                {
                    string input = Console.ReadLine();
                    if (input?.Equals("q", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        break;
                    }
                } while (true);

                LogService.WriteText($"{DateTime.Now} 正在关闭服务,请耐心等待");
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex, "启动服务异常");
            }
            finally
            {
                if (channel != null)
                {
                    await channel.CloseAsync();
                }

                if (_client != null)
                {
                    await _client.ShutdownGracefullyAsync();
                }

                if (_acceptor != null)
                {
                    await _acceptor.ShutdownGracefullyAsync();
                }
                LogService.WriteText($"{DateTime.Now} 服务已关闭!");
            }
        }
    }
}
