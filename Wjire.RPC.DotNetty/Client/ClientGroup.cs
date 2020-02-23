using System;
using System.Collections.Concurrent;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using ImpromptuInterface;
using Microsoft.Extensions.ObjectPool;
using Wjire.Log;

namespace Wjire.RPC.DotNetty.Client
{
    public class ClientGroup
    {

        private readonly ClientConfig _config;
        private readonly ObjectPool<IChannel> _pool;
        private readonly ClientInvoker _clientInvoker = new ClientInvoker();
        private readonly ConcurrentDictionary<Type, Lazy<object>> _clients = new ConcurrentDictionary<Type, Lazy<object>>();

        public ClientGroup(ClientConfig config)
        {
            Console.WriteLine("ClientGroup ctor");
            _config = config;
            IEventLoopGroup group = new MultithreadEventLoopGroup();
            try
            {
                Bootstrap bootstrap = CreateBootstrap(group);
                _pool = CreateChannelPool(bootstrap);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex, "ClientGroup ctor throw a exception");
                group.ShutdownGracefullyAsync().Wait();
                throw;
            }
        }

        private Bootstrap CreateBootstrap(IEventLoopGroup group)
        {
            ClientHandler handler = new ClientHandler(_clientInvoker);
            Bootstrap bootstrap = new Bootstrap()
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.SoSndbuf, _config.SoSndbuf)
                .Option(ChannelOption.SoRcvbuf, _config.SoRcvbuf)
                .Option(ChannelOption.SoReuseaddr, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    //pipeline.AddLast(new IdleStateHandler(0, 0, _config.AllIdleTimeSeconds));
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(4));
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                    pipeline.AddLast(handler);
                }));
            return bootstrap;
        }


        private ObjectPool<IChannel> CreateChannelPool(Bootstrap bootstrap)
        {
            ChannelPooledObjectPolicy policy = new ChannelPooledObjectPolicy(bootstrap, _config.RemoteAddress);
            return new DefaultObjectPool<IChannel>(policy, _config.PooledObjectMax);
        }


        internal T GetClient<T>() where T : class
        {
            Type type = typeof(T);
            Lazy<object> client = _clients.GetOrAdd(type, k =>
            {
                return new Lazy<object>(() =>
                {
                    Client c = new Client(type, _config, _pool, _clientInvoker);
                    return c.ActLike<T>();
                });
            });
            return (T)client.Value;
        }
    }
}
