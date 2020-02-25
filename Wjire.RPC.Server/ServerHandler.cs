using System;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;

namespace Wjire.RPC.Server
{

    internal class ServerHandler : SimpleChannelInboundHandler<IByteBuffer>
    {
        private readonly MessageHandler _messageHandler;
        private readonly ILogger<ServerHandler> _logger;
        private const string HEARTBEAT = "HEARTBEAT";
        public ServerHandler(MessageHandler messageHandler, ILogger<ServerHandler> logger)
        {
            _messageHandler = messageHandler;
            _logger = logger;
        }

        public override bool IsSharable => true;

        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            byte[] bytes = new byte[msg.ReadableBytes];
            msg.ReadBytes(bytes);
            byte[] buffer = _messageHandler.GetResponseBytes(bytes);
            ctx.WriteAndFlushAsync(Unpooled.WrappedBuffer(buffer));
        }


        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            _logger.ServerHandlerCaughtException(exception);
            context.CloseAsync();
        }


        //protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        //{
        //    Console.WriteLine(ctx.Channel.RemoteAddress);
        //    string requestString = msg.ToString(Encoding.UTF8);
        //    if (requestString.Equals(HEARTBEAT))
        //    {
        //        Console.WriteLine("客户端发来的心跳检测,开始关闭连接");
        //        ctx.Channel.CloseAsync();
        //    }
        //    else
        //    {
        //        byte[] buffer = _messageHandler.GetResponseBytes(requestString);
        //        IByteBuffer wrappedBuffer = Unpooled.WrappedBuffer(buffer);
        //        ctx.WriteAndFlushAsync(wrappedBuffer);
        //    }
        //}
    }
}
