using System;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Wjire.RPC.DotNetty.Serializer;

namespace Wjire.RPC.DotNetty.Server
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
                Console.WriteLine($"{DateTime.Now} 开始启动服务!");
                ServerHandler handler = new ServerHandler(_messageHandler);
                _acceptor = new MultithreadEventLoopGroup(1);
                _client = new MultithreadEventLoopGroup();
                // 服务器引导程序
                _bootstrap = new ServerBootstrap()
                    .Group(_acceptor, _client)
                    .Channel<TcpServerSocketChannel>()
                    .Option(ChannelOption.SoBacklog, 1024)
                    .Option(ChannelOption.SoSndbuf, 32 * 1024)
                    .Option(ChannelOption.SoRcvbuf, 32 * 1024)
                    .Option(ChannelOption.SoReuseaddr, true)
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        pipeline.AddLast(handler);
                    }));
            }
            catch (Exception)
            {
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
                channel = await _bootstrap.BindAsync(_port);
                Console.WriteLine($"{DateTime.Now} 服务已启动,端口号 : {_port},按 'Q' 键退出");
                do
                {
                    var input = Console.ReadLine();
                    if (input?.Equals("q", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        break;
                    }
                } while (true);
                Console.WriteLine($"{DateTime.Now} 正在关闭服务,请耐心等待");
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
                Console.WriteLine($"{DateTime.Now} 服务已关闭!");
            }
        }
    }
}
