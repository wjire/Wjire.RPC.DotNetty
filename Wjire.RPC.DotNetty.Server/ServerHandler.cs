using System;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Wjire.RPC.DotNetty.Server
{
    public class ServerHandler : ChannelHandlerAdapter
    {
        private readonly MessageHandler _messageHandler;

        public ServerHandler(MessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var byteBuffer = message as IByteBuffer;
            var msg = byteBuffer.ToString(Encoding.UTF8);
            var buffer = _messageHandler.GetResponseBytes(msg);
            IByteBuffer wrappedBuffer = Unpooled.WrappedBuffer(buffer);
            context.WriteAndFlushAsync(wrappedBuffer);
            context.CloseAsync();
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("ServerHandler throw Exception: " + exception);
            context.CloseAsync();
        }
    }
}
