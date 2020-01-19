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
        private readonly MessageHandler _messageHandler = new MessageHandler();

        public Server(int port)
        {
            try
            {
                Console.WriteLine($"{DateTime.Now} 开始创建服务!");
                _port = port;
                _acceptor = new MultithreadEventLoopGroup(1);
                _client = new MultithreadEventLoopGroup();
                // 服务器引导程序
                _bootstrap = new ServerBootstrap()
                    .Group(_acceptor, _client)
                    .Channel<TcpServerSocketChannel>()
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        var pipeline = channel.Pipeline;
                        pipeline.AddLast(new ServerHandler(_messageHandler));
                    }));
            }
            catch (Exception)
            {
                //执行很慢.
                Task.WaitAll(_client?.ShutdownGracefullyAsync(), _acceptor?.ShutdownGracefullyAsync());
                _client = null;
                _acceptor = null;
                Console.WriteLine("创建服务发生异常!");
            }
        }

        public Server RegisterServices(ServiceCollection services)
        {
            _messageHandler.InitServicesMap(services);
            return this;
        }


        public Server RegisterSerializer(ISerializer serializer)
        {
            _messageHandler.Serializer = serializer;
            return this;
        }

        public async Task Start()
        {
            IChannel channel = null;
            try
            {
                channel = await _bootstrap.BindAsync(_port);
                Console.WriteLine($"{DateTime.Now} 服务已启动,端口号 : {_port},按任意键退出");
                Console.ReadLine();
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
