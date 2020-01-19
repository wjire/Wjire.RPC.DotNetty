using System;
using System.Collections.Concurrent;

namespace Wjire.RPC.DotNetty.Client
{
    internal class MessageHandler
    {
        private readonly ConcurrentDictionary<string, MessageWaiter> _messages = new ConcurrentDictionary<string, MessageWaiter>();
        internal void ReadyToWait(string channelId,out MessageWaiter waiter)
        {
            waiter = new MessageWaiter();
            _messages[channelId] = waiter;
        }

        internal void WaitResponse(MessageWaiter waiter,TimeSpan timeOut)
        {
            waiter.WaitResponse(timeOut);
        }


        internal void SetResponseCompleted(string channelId, string responseString)
        {
            var waiter = _messages[channelId];
            waiter.SetResponseCompleted(responseString);
        }

        internal void RemoveWaiter(string channelId)
        {
            _messages.TryRemove(channelId, out var waiter);
        }
    }
}
