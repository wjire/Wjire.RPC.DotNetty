﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Wjire.RPC.DotNetty.Model;
using Wjire.RPC.DotNetty.Serializer;

namespace Wjire.RPC.DotNetty.Client
{
    internal class ClientInvoker
    {
        private readonly ISerializer _serializer;

        private readonly ConcurrentDictionary<string, ClientWaiter> _waiters = new ConcurrentDictionary<string, ClientWaiter>();

        internal ClientInvoker() : this(RpcConfig.DefaultSerializer) { }

        internal ClientInvoker(ISerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }


        internal string GetChannelId(IChannel channel)
        {
            return channel.Id.AsLongText();
        }

        internal void Set(IChannel channel, byte[] bytes)
        {
            string channelId = GetChannelId(channel);
            _waiters.TryRemove(channelId, out ClientWaiter waiter);
            waiter.Set(bytes);
        }


        internal object GetResponse(IChannel channel, Type serviceType, Request request, TimeSpan timeOut)
        {
            string channelId = GetChannelId(channel);
            ClientWaiter messageWaiter = new ClientWaiter(timeOut);
            try
            {
                _waiters[channelId] = messageWaiter;
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
                _waiters.TryRemove(channelId, out ClientWaiter value);
                throw;
            }
            finally
            {
                messageWaiter.Dispose();
            }
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
