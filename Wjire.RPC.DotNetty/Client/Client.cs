using System;
using System.Dynamic;
using System.Net;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.ObjectPool;
using Wjire.RPC.DotNetty.Helper;
using Wjire.RPC.DotNetty.Log;
using Wjire.RPC.DotNetty.Model;

namespace Wjire.RPC.DotNetty.Client
{
    public class Client : DynamicObject
    {
        private readonly Type _serviceType;
        private readonly ClientConfig _config;
        private readonly ClientInvoker _clientInvoker = new ClientInvoker();

        public Client(Type serviceType, ClientConfig config)
        {
            _serviceType = serviceType;
            _config = config;
            IEventLoopGroup group = null;
            try
            {
                Console.WriteLine("ctor Client");
                Bootstrap bootstrap = InitBootstrap(out group);
                _clientInvoker.ChannelPool = InitChannelPool(bootstrap);
            }
            catch (Exception ex)
            {
                RpcLogService.WriteLog(ex, "Client ctor");
                group?.ShutdownGracefullyAsync().Wait();
                throw;
            }
        }


        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            RpcRequest request = new RpcRequest
            {
                MethodName = binder.Name,
                Arguments = args,
                ServiceName = _serviceType.FullName
            };
            result = _clientInvoker.GetResponse(_serviceType, request, _config.TimeOut);
            return true;
        }


        private Bootstrap InitBootstrap(out IEventLoopGroup group)
        {
            group = new MultithreadEventLoopGroup();
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
                    pipeline.AddLast(handler);
                }));
            return bootstrap;
        }


        private ObjectPool<IChannel> InitChannelPool(Bootstrap bootstrap)
        {
            ChannelPooledObjectPolicy policy = new ChannelPooledObjectPolicy(bootstrap, _config.RemoteAddress);
            DefaultObjectPool<IChannel> pool = new DefaultObjectPool<IChannel>(policy, _config.PooledObjectMax);
            return pool;
        }


        private class ChannelPooledObjectPolicy : IPooledObjectPolicy<IChannel>
        {
            private readonly Bootstrap _bootstrap;
            private readonly IPEndPoint _remoteAddress;

            public ChannelPooledObjectPolicy(Bootstrap bootstrap, IPEndPoint remoteAddress)
            {
                _bootstrap = bootstrap;
                _remoteAddress = remoteAddress;
            }

            public IChannel Create()
            {
                //return AsyncHelper1.RunSync(() => _bootstrap.ConnectAsync(_remoteAddress));
                return AsyncHelper2.RunSync(() => _bootstrap.ConnectAsync(_remoteAddress));
            }

            public bool Return(IChannel obj)
            {
                return true;
            }
        }
    }
}
