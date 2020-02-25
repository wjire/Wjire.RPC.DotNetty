using System;
using System.Dynamic;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.ObjectPool;
using Wjire.RPC.Model;
using Wjire.RPC.Serializer;

namespace Wjire.RPC.Client
{
    public class Client : DynamicObject
    {
        private readonly Type _serviceType;
        private readonly TimeSpan _timeOut;
        private readonly ObjectPool<IChannel> _channelPool;
        private readonly ClientInvoker _clientInvoker;
        private readonly IRpcSerializer _rpcSerializer;


        internal Client(Type serviceType, ClientConfig config, ObjectPool<IChannel> channelPool, ClientInvoker clientInvoker)
        {
            //Console.WriteLine("Client ctor");
            _serviceType = serviceType;
            _timeOut = TimeSpan.FromSeconds(config.TimeOut);
            _rpcSerializer = config.RpcSerializer;
            _channelPool = channelPool;
            _clientInvoker = clientInvoker;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            RpcRequest request = new RpcRequest
            {
                MethodName = binder.Name,
                Arguments = args,
                ServiceContractFullName = _serviceType.FullName,
            };
            result = GetReturnValue(request);
            return true;
        }


        private object GetReturnValue(RpcRequest request)
        {
            ClientWaiter waiter = new ClientWaiter(_timeOut);
            IChannel channel = CreateChannel();
            try
            {
                RpcResponse response = GerResponseFromServer(channel, request, waiter);
                return GetMethodResult(request.MethodName, response);
            }
            catch (OperationCanceledException)
            {
                RemoveWaiter(channel);
                channel?.CloseAsync();
                throw;
            }
            catch (Exception)
            {
                RemoveWaiter(channel);
                throw;
            }
            finally
            {
                waiter.Dispose();
            }
        }

        private IChannel CreateChannel()
        {
            IChannel channel;
            while ((channel = _channelPool.Get()).Open == false) { }
            return channel;
        }


        private RpcResponse GerResponseFromServer(IChannel channel, RpcRequest request, ClientWaiter waiter)
        {
            _clientInvoker.Add(channel, waiter);
            IByteBuffer buffer = Unpooled.WrappedBuffer(_rpcSerializer.ToBytes(request));
            channel.WriteAndFlushAsync(buffer);
            waiter.Wait();
            _channelPool.Return(channel);
            return _rpcSerializer.ToObject<RpcResponse>(waiter.Bytes);
        }


        private object GetMethodResult(string methodName, RpcResponse response)
        {
            if (response.Success == false)
            {
                throw new Exception(response.Message);
            }
            Type returnType = _serviceType.GetMethod(methodName)?.ReturnType;
            return returnType == typeof(void) ? null : _rpcSerializer.ToObject(response.Data, returnType);
        }

        private void RemoveWaiter(IChannel channel)
        {
            bool removeResult = _clientInvoker.Remove(channel, out ClientWaiter waiter);
        }
    }
}
