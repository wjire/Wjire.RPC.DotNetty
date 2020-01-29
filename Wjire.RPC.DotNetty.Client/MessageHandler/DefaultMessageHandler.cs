using System;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Wjire.RPC.DotNetty.Common;
using Wjire.RPC.DotNetty.Model;

namespace Wjire.RPC.DotNetty.Client
{
    internal class DefaultMessageHandler : AbstractMessageHandler
    {
        private readonly ISerializer _serializer;

        internal DefaultMessageHandler() : this(new RJsonSerializer()) { }

        internal DefaultMessageHandler(ISerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public override object GetResponse(IChannel channel, Type serviceType, Request request, TimeSpan timeOut)
        {
            string channelId = GetChannelId(channel);
            ClientWaiter messageWaiter = new ClientWaiter(timeOut);
            try
            {
                Waiters[channelId] = messageWaiter;
                var buffer = Unpooled.WrappedBuffer(_serializer.ToBytes(request));
                channel.WriteAndFlushAsync(buffer);
                messageWaiter.Wait();
                Response response = _serializer.ToObject<Response>(messageWaiter.Bytes);
                if (response.Success == false)
                {
                    throw new Exception(response.Message);
                }
                Type returnType = serviceType.GetMethod(request.MethodName).ReturnType;
                object result = returnType == typeof(void) ? null : _serializer.ToObject(response.Data, returnType);
                return result;
            }
            catch (Exception)
            {
                Waiters.TryRemove(channelId, out ClientWaiter value);
                throw;
            }
            finally
            {
                messageWaiter.Dispose();
            }
        }
    }
}
