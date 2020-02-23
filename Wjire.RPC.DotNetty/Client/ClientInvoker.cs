using System.Collections.Concurrent;
using DotNetty.Transport.Channels;
using Wjire.RPC.DotNetty.Extension;

namespace Wjire.RPC.DotNetty.Client
{
    internal class ClientInvoker
    {
        private readonly ConcurrentDictionary<string, ClientWaiter> _waiters = new ConcurrentDictionary<string, ClientWaiter>();

        internal void Add(IChannel channel, ClientWaiter waiter)
        {
            _waiters[channel.GetId()] = waiter;
        }

        internal bool Remove(IChannel channel, out string channelId)
        {
            channelId = channel.GetId();
            return _waiters.TryRemove(channelId, out ClientWaiter waiter);
        }


        internal void Set(IChannel channel, byte[] bytes)
        {
            string channelId = channel.GetId();
            _waiters.TryRemove(channelId, out ClientWaiter waiter);
            waiter.Set(bytes);
        }
    }
}
