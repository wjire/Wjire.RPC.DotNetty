using System;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Wjire.RPC.Common.Message;
using Wjire.RPC.Common.Serializer;

namespace Wjire.RPC.Client
{
    internal class ClientHandler : SimpleChannelInboundHandler<IByteBuffer>
    {
        private readonly ClientInvoker _clientInvoker;
        private readonly IRpcSerializer _rpcSerializer;
        private const string HEARTBEAT = "HEARTBEAT";

        internal ClientHandler(ClientInvoker clientInvoker, IRpcSerializer rpcSerializer)
        {
            _clientInvoker = clientInvoker;
            _rpcSerializer = rpcSerializer;
        }

        public override bool IsSharable => true;

        protected override void ChannelRead0(IChannelHandlerContext context, IByteBuffer message)
        {
            byte[] bytes = new byte[message.ReadableBytes];
            message.ReadBytes(bytes);
            _clientInvoker.Set(context.Channel, bytes);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            RpcResponse rpcResponse = new RpcResponse
            {
                Message = exception.ToString(),
            };
            byte[] bytes = _rpcSerializer.ToBytes(rpcResponse);
            _clientInvoker.Set(context.Channel, bytes);
            context.CloseAsync();
        }
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
}