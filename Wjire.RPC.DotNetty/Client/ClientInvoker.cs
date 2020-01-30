using System;
using System.Collections.Concurrent;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Wjire.RPC.DotNetty.Model;
using Wjire.RPC.DotNetty.Serializer;

namespace Wjire.RPC.DotNetty.Client
{
    internal class ClientInvoker
    {
        private readonly ISerializer _serializer;

        protected readonly ConcurrentDictionary<string, ClientWaiter> Waiters = new ConcurrentDictionary<string, ClientWaiter>();

        internal ClientInvoker() : this(RpcConfig.DefaultSerializer) { }

        internal ClientInvoker(ISerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }


        public string GetChannelId(IChannel channel)
        {
            return channel.Id.AsLongText();
        }

        public void Set(IChannel channel, byte[] bytes)
        {
            string channelId = GetChannelId(channel);
            Waiters.TryRemove(channelId, out ClientWaiter waiter);
            waiter.Set(bytes);
        }


        public object GetResponse(IChannel channel, Type serviceType, Request request, TimeSpan timeOut)
        {
            string channelId = GetChannelId(channel);
            ClientWaiter messageWaiter = new ClientWaiter(timeOut);
            try
            {
                Waiters[channelId] = messageWaiter;
                IByteBuffer buffer = Unpooled.WrappedBuffer(_serializer.ToBytes(request));
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
