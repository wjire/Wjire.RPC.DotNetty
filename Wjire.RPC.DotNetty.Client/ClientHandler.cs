using System;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Wjire.RPC.DotNetty.Client
{
    internal class ClientHandler : ChannelHandlerAdapter
    {
        private readonly MessageHandler _messageHandler;

        internal ClientHandler(MessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = message as IByteBuffer;
            var msg = buffer.ToString(Encoding.UTF8);
            string channelId = context.Channel.Id.AsLongText();
            _messageHandler.SetResponseCompleted(channelId, msg);
            context.CloseAsync();
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }


        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("ClientHandler throw Exception: " + exception);
            context.CloseAsync();
        }
    }
}
