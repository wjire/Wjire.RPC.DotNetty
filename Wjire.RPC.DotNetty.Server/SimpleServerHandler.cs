using System;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Wjire.RPC.DotNetty.Server
{
    //Netty 建议服务端不要用这个类.暂时不明白

    public class SimpleServerHandler : SimpleChannelInboundHandler<IByteBuffer>
    {
        private readonly MessageHandler _messageHandler;
        private const string HEARTBEAT = "HEARTBEAT";
        public SimpleServerHandler(MessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public override bool IsSharable => true;

        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            Console.WriteLine(ctx.Channel.RemoteAddress);
            string requestString = msg.ToString(Encoding.UTF8);
            if (requestString.Equals(HEARTBEAT))
            {
                Console.WriteLine("客户端发来的心跳检测,开始关闭连接");
                ctx.Channel.CloseAsync();
            }
            else
            {
                byte[] buffer = _messageHandler.GetResponseBytes(requestString);
                IByteBuffer wrappedBuffer = Unpooled.WrappedBuffer(buffer);
                ctx.WriteAndFlushAsync(wrappedBuffer);
            }
        }


        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("ServerHandler throw Exception: " + exception);
        }
    }
}
