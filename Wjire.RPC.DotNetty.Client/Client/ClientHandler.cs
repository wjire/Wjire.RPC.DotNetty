using System;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;

namespace Wjire.RPC.DotNetty.Client
{
    internal class ClientHandler : SimpleChannelInboundHandler<IByteBuffer>
    {
        private readonly IMessageHandler _messageHandler;
        private const string HEARTBEAT = "HEARTBEAT";

        internal ClientHandler(IMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public override bool IsSharable => true;

        protected override void ChannelRead0(IChannelHandlerContext context, IByteBuffer message)
        {
            var bytes = new byte[message.ReadableBytes];
            message.ReadBytes(bytes);
            _messageHandler.Set(context.Channel, bytes);
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (evt is IdleStateEvent)
            {
                Console.WriteLine(_messageHandler.GetChannelId(context.Channel) + " 连接空闲太久,开始关闭");
                context.WriteAndFlushAsync(Unpooled.UnreleasableBuffer(Unpooled.CopiedBuffer(Encoding.UTF8.GetBytes(HEARTBEAT)))).ContinueWith(
                    t =>
                    {
                        if (t.IsFaulted)
                        {
                            context.CloseAsync();
                        }
                    });
            }
            else
            {
                base.UserEventTriggered(context, evt);
            }
        }


        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("ClientHandler throw Exception: " + exception);
            context.CloseAsync();
        }
    }
}
