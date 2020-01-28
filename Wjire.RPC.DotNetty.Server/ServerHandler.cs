using System;
using System.Text;
using System.Threading;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Wjire.RPC.DotNetty.Server
{
    public class ServerHandler : ChannelHandlerAdapter
    {
        private readonly MessageHandler _messageHandler;

        private static int count;

        public override bool IsSharable => true;

        public ServerHandler(MessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
            Console.WriteLine($"new ServerHandler({Interlocked.Increment(ref count)})");
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            IByteBuffer byteBuffer = message as IByteBuffer;
            string msg = byteBuffer.ToString(Encoding.UTF8);
            byte[] buffer = _messageHandler.GetResponseBytes(msg);
            IByteBuffer wrappedBuffer = Unpooled.WrappedBuffer(buffer);
            //context.WriteAndFlushAsync(wrappedBuffer);
            context.WriteAsync(wrappedBuffer);
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
