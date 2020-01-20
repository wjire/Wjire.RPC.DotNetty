using System;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Wjire.RPC.DotNetty.Client
{
    internal class SimpleClientHandler : SimpleChannelInboundHandler<IByteBuffer>
    {
        private readonly MessageHandler _messageHandler;

        internal SimpleClientHandler(MessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public override bool IsSharable => true;

        protected override void ChannelRead0(IChannelHandlerContext context, IByteBuffer message)
        {
            var msg = message.ToString(Encoding.UTF8);
            string channelId = context.Channel.Id.AsLongText();
            _messageHandler.SetResponseCompleted(channelId, msg);
        }


        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.CloseAsync();
        }


        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
        }


        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("ClientHandler throw Exception: " + exception);
            context.CloseAsync();
        }
    }
}
