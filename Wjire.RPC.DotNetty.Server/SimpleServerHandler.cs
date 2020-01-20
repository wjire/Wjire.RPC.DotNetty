using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Wjire.RPC.DotNetty.Model;

namespace Wjire.RPC.DotNetty.Server
{
    //Netty 建议服务端不要用这个类

    public class SimpleServerHandler : SimpleChannelInboundHandler<IByteBuffer>
    {
        private readonly MessageHandler _messageHandler;

        public SimpleServerHandler(MessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public override bool IsSharable => true;

        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            var requestString = msg.ToString(Encoding.UTF8);
            var buffer = _messageHandler.GetResponseBytes(requestString);
            IByteBuffer wrappedBuffer = Unpooled.WrappedBuffer(buffer);
            ctx.WriteAsync(wrappedBuffer);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
            context.CloseAsync();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("ServerHandler throw Exception: " + exception);
            context.CloseAsync();
        }
    }
}
