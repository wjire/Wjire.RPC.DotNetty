using System.Collections.Concurrent;
using DotNetty.Transport.Channels;
using Wjire.RPC.Client.Extension;

namespace Wjire.RPC.Client
{
    internal class ClientInvoker
    {
        private readonly ConcurrentDictionary<string, ClientWaiter> _waiters = new ConcurrentDictionary<string, ClientWaiter>();
        internal void Add(IChannel channel, ClientWaiter waiter)
        {
            _waiters[channel.GetId()] = waiter;
        }

        internal void Set(IChannel channel, byte[] bytes)
        {
            Remove(channel, out ClientWaiter waiter);
            waiter.Set(bytes);
        }

        internal bool Remove(IChannel channel, out ClientWaiter waiter)
        {
            string channelId = channel.GetId();
            return _waiters.TryRemove(channelId, out waiter);
        }
    }
}
