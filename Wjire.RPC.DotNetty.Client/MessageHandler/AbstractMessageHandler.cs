using System;
using System.Collections.Concurrent;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Wjire.RPC.DotNetty.Model;

namespace Wjire.RPC.DotNetty.Client
{
    internal abstract class AbstractMessageHandler : IMessageHandler
    {
        protected readonly ConcurrentDictionary<string, ClientWaiter> Waiters = new ConcurrentDictionary<string, ClientWaiter>();

        public void Set(IChannel channel, IByteBuffer byteBuffer)
        {
            string channelId = GetChannelId(channel);
            Waiters.TryRemove(channelId, out ClientWaiter waiter);
            waiter.Set(byteBuffer);
            waiter.ByteBuffer.Retain();
        }

        public abstract object GetResponse(IChannel channel, Type serviceType, Request request, TimeSpan timeOut);

        protected abstract string GetChannelId(IChannel channel);
    }
}
