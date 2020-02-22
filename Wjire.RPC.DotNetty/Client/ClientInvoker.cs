using System;
using System.Collections.Concurrent;
using System.Threading;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.ObjectPool;
using Wjire.Log;
using Wjire.RPC.DotNetty.Model;
using Wjire.RPC.DotNetty.Serializer;

namespace Wjire.RPC.DotNetty.Client
{
    internal class ClientInvoker
    {
        private readonly IRpcSerializer _serializer;
        internal ObjectPool<IChannel> ChannelPool { get; set; }
        private readonly ConcurrentDictionary<string, ClientWaiter> _waiters = new ConcurrentDictionary<string, ClientWaiter>();

        internal ClientInvoker(IRpcSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }


        internal object GetResponse(Type serviceType, RpcRequest request, TimeSpan timeOut)
        {
            IChannel channel;
            while ((channel = ChannelPool.Get()).Open == false) { }
            string channelId = GetChannelId(channel);
            ClientWaiter messageWaiter = new ClientWaiter(timeOut);
            RpcResponse response = null;
            try
            {
                _waiters[channelId] = messageWaiter;
                IByteBuffer buffer = Unpooled.WrappedBuffer(_serializer.ToBytes(request));
                channel.WriteAndFlushAsync(buffer);
                messageWaiter.Wait();
                ChannelPool.Return(channel);
                response = _serializer.ToObject<RpcResponse>(messageWaiter.Bytes);
                if (response.Success == false)
                {
                    throw new Exception(response.Message);
                }
                Type returnType = serviceType.GetMethod(request.MethodName).ReturnType;
                object result = returnType == typeof(void) ? null : _serializer.ToObject(response.Data, returnType);
                return result;
            }
            catch (OperationCanceledException ex)
            {
                channel?.CloseAsync();
                LogService.WriteExceptionAsync(ex, "服务器响应超时", request, response);
                throw;
            }
            catch (Exception ex)
            {
                _waiters.TryRemove(channelId, out ClientWaiter value);
                LogService.WriteExceptionAsync(ex, "服务器出现异常", request, response);
                throw;
            }
            finally
            {
                messageWaiter.Dispose();
            }
        }

        private string GetChannelId(IChannel channel)
        {
            return channel.Id.AsLongText();
        }

        internal void Set(IChannel channel, byte[] bytes)
        {
            string channelId = GetChannelId(channel);
            _waiters.TryRemove(channelId, out ClientWaiter waiter);
            waiter.Set(bytes);
        }

        private class ClientWaiter : IDisposable
        {
            private readonly TimeSpan _timeOut;
            internal byte[] Bytes { get; set; }

            private readonly ManualResetEventSlim _mutex = new ManualResetEventSlim();


            internal ClientWaiter(TimeSpan timeOut)
            {
                _timeOut = timeOut;
            }

            internal void Wait()
            {
                _mutex.Wait(new CancellationTokenSource(_timeOut).Token);
            }

            internal void Set(byte[] bytes)
            {
                Bytes = bytes;
                _mutex.Set();
            }

            public void Dispose()
            {
                _mutex?.Dispose();
            }
        }
    }
}
