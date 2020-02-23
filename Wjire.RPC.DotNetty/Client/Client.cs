using System;
using System.Dynamic;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.ObjectPool;
using Wjire.Log;
using Wjire.RPC.DotNetty.Model;
using Wjire.RPC.DotNetty.Serializer;

namespace Wjire.RPC.DotNetty.Client
{
    public class Client : DynamicObject
    {
        private readonly Type _serviceType;
        private readonly TimeSpan _timeOut;
        private readonly IRpcSerializer _rpcSerializer;
        private readonly ObjectPool<IChannel> _channelPool;
        private readonly ClientInvoker _clientInvoker;


        internal Client(Type serviceType, ClientConfig config, ObjectPool<IChannel> channelPool, ClientInvoker clientInvoker)
        {
            //Console.WriteLine("Client ctor");
            _serviceType = serviceType;
            _timeOut = config.TimeOut;
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
                ServiceType = _serviceType,
            };
            result = GetReturnValue(request);
            return true;
        }


        private object GetReturnValue(RpcRequest request)
        {
            ClientWaiter waiter = new ClientWaiter(_timeOut);
            RpcResponse response = null;
            IChannel channel = CreateChannel();
            try
            {
                response = GerResponseFromServer(channel, request, waiter);
                return GetMethodResult(request.MethodName, response);
            }
            catch (OperationCanceledException ex)
            {
                channel?.CloseAsync();
                LogService.WriteExceptionAsync(ex, "服务器响应超时", request, response);
                throw;
            }
            catch (Exception ex)
            {
                LogService.WriteExceptionAsync(ex, "服务器出现异常", request, response);
                bool removeResult = _clientInvoker.Remove(channel, out string channelId);
                if (removeResult == false)
                {
                    LogService.WriteExceptionAsync(new Exception("移除 channel 失败"), "remove channel", channelId);
                }
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
    }
}
