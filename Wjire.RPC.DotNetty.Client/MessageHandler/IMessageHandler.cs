using System;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Wjire.RPC.DotNetty.Model;

namespace Wjire.RPC.DotNetty.Client
{
    internal interface IMessageHandler
    {
        object GetResponse(IChannel channel, Type serviceType, Request request, TimeSpan timeOut);

        void Set(IChannel channel, IByteBuffer byteBuffer);

        string GetChannelId(IChannel channel);
    }
}
