using System;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Wjire.Log;

namespace Wjire.RPC.DotNetty
{

    internal class ServerHandler : SimpleChannelInboundHandler<IByteBuffer>
    {
        private readonly ServerInvoker _serverInvoker;
        private const string HEARTBEAT = "HEARTBEAT";
        internal ServerHandler(ServerInvoker serverInvoker)
        {
            _serverInvoker = serverInvoker;
        }

        public override bool IsSharable => true;

        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            Console.WriteLine(ctx.Channel.RemoteAddress);
            byte[] bytes = new byte[msg.ReadableBytes];
            msg.ReadBytes(bytes);
            byte[] buffer = _serverInvoker.GetResponseBytes(bytes);
            ctx.WriteAndFlushAsync(Unpooled.WrappedBuffer(buffer));
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
        //        byte[] buffer = _serverInvoker.GetResponseBytes(requestString);
        //        IByteBuffer wrappedBuffer = Unpooled.WrappedBuffer(buffer);
        //        ctx.WriteAndFlushAsync(wrappedBuffer);
        //    }
        //}



        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            LogService.WriteException(exception, "ServerHandler throw Exception: ");
            context.CloseAsync();
        }
    }
}
