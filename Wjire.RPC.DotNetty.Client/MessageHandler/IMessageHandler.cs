using System;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Wjire.RPC.DotNetty.Model;

namespace Wjire.RPC.DotNetty.Client
{
    internal interface IMessageHandler
    {
        object GetResponse(IChannel channel, Type serviceType, Request request, TimeSpan timeOut);
        
        string GetChannelId(IChannel channel);

        void Set(IChannel channel, byte[] bytes);
    }
}
