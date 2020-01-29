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

        public virtual string GetChannelId(IChannel channel)
        {
            return channel.Id.AsLongText();
        }

        public virtual void Set(IChannel channel, byte[] bytes)
        {
            string channelId = GetChannelId(channel);
            Waiters.TryRemove(channelId, out ClientWaiter waiter);
            waiter.Set(bytes);
        }

        public abstract object GetResponse(IChannel channel, Type serviceType, Request request, TimeSpan timeOut);
    }
}
