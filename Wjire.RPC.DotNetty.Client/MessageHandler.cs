using System;
using System.Collections.Concurrent;
using Wjire.RPC.DotNetty.Model;
using Wjire.RPC.DotNetty.Serializer;

namespace Wjire.RPC.DotNetty.Client
{
    internal class MessageHandler
    {
        private readonly ConcurrentDictionary<string, MessageWaiter> _messages = new ConcurrentDictionary<string, MessageWaiter>();
        internal void ReadyToWait(string channelId, TimeSpan timeOut, out MessageWaiter waiter)
        {
            waiter = new MessageWaiter(timeOut);
            _messages[channelId] = waiter;
        }
        
        internal void WaitResponse(MessageWaiter waiter)
        {
            waiter.WaitResponse();
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
