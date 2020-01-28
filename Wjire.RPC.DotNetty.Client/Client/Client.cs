﻿using System;
using System.Dynamic;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.ObjectPool;
using Wjire.RPC.DotNetty.Client.ChannelPool;
using Wjire.RPC.DotNetty.Model;

namespace Wjire.RPC.DotNetty.Client
{
    public class Client : DynamicObject
    {
        private readonly Type _serviceType;
        private readonly ClientConfig _config;
        private readonly Bootstrap _bootstrap;
        private readonly ObjectPool<IChannel> _channelPool;
        private readonly IMessageHandler _messageHandler = new DefaultMessageHandler();

        public Client(Type serviceType, ClientConfig config)
        {
            _serviceType = serviceType;
            _config = config;
            IEventLoopGroup group = null;
            try
            {
                Console.WriteLine("ctor Client");
                _bootstrap = InitBootstrap(out group);
                _channelPool = InitChannelPool();
            }
            catch (Exception)
            {
                group?.ShutdownGracefullyAsync().Wait();
                throw;
            }
        }


        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            IChannel channel = null;
            try
            {
                Request request = new Request
                {
                    MethodName = binder.Name,
                    Arguments = args,
                    ServiceName = _serviceType.FullName
                };
                while ((channel = GetChannel()).Open == false)
                {
                }

                result = _messageHandler.GetResponse(channel, _serviceType, request, _config.TimeOut);
                _channelPool.Return(channel);
                return true;
            }
            catch (OperationCanceledException)
            {
                channel?.CloseAsync();
                throw;
            }
        }

        private IChannel GetChannel()
        {
            return _channelPool.Get();
        }


        private Bootstrap InitBootstrap(out IEventLoopGroup group)
        {
            group = new MultithreadEventLoopGroup();
            ClientHandler handler = new ClientHandler(_messageHandler);
            Bootstrap bootstrap = new Bootstrap()
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast(new IdleStateHandler(0, 0, _config.AllIdleTimeSeconds));
                    pipeline.AddLast(handler);
                }));
            return bootstrap;
        }


        private ObjectPool<IChannel> InitChannelPool()
        {
            ChannelPooledObjectPolicy policy = new ChannelPooledObjectPolicy(_bootstrap, _config.RemoteAddress);
            DefaultObjectPool<IChannel> pool = new DefaultObjectPool<IChannel>(policy, _config.PooledObjectMax);
            return pool;
        }
    }
}