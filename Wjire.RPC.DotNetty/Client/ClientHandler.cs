using System;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Wjire.RPC.DotNetty.Log;

namespace Wjire.RPC.DotNetty.Client
{
    internal class ClientHandler : SimpleChannelInboundHandler<IByteBuffer>
    {
        private readonly ClientInvoker _clientInvoker;
        private const string HEARTBEAT = "HEARTBEAT";

        internal ClientHandler(ClientInvoker clientInvoker)
        {
            _clientInvoker = clientInvoker;
        }

        public override bool IsSharable => true;

        protected override void ChannelRead0(IChannelHandlerContext context, IByteBuffer message)
        {
            byte[] bytes = new byte[message.ReadableBytes];
            message.ReadBytes(bytes);
            _clientInvoker.Set(context.Channel, bytes);
        }

        //public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        //{
        //    if (evt is IdleStateEvent)
        //    {
        //        Console.WriteLine(_clientInvoker.GetChannelId(context.Channel) + " 连接空闲太久,开始关闭");
        //        context.WriteAndFlushAsync(Unpooled.UnreleasableBuffer(Unpooled.CopiedBuffer(Encoding.UTF8.GetBytes(HEARTBEAT)))).ContinueWith(
        //            t =>
        //            {
        //                if (t.IsFaulted)
        //                {
        //                    context.CloseAsync();
        //                }
        //            });
        //    }
        //    else
        //    {
        //        base.UserEventTriggered(context, evt);
        //    }
        //}


        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            RpcLogService.WriteLog(exception, "ClientHandler throw Exception");
            context.CloseAsync();
        }
    }
}
