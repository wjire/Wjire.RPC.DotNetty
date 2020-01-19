using System;
using System.Dynamic;
using System.Net;
using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Wjire.RPC.DotNetty.Model;
using Wjire.RPC.DotNetty.Serializer;

namespace Wjire.RPC.DotNetty.Client
{
    public class Client : DynamicObject
    {
        public IPEndPoint IPEndPoint { get; set; }

        public Type ServiceType { get; set; }

        public TimeSpan TimeOut { get; set; }

        private readonly Bootstrap _bootstrap;

        private readonly MessageHandler _messageHandler = new MessageHandler();

        private ISerializer _serializer = new JsonSerializer();

        private readonly IEventLoopGroup _group = new MultithreadEventLoopGroup();

        public Client()
        {
            try
            {
                Console.WriteLine("new Client()");
                _bootstrap = new Bootstrap()
                    .Group(_group)
                    .Channel<TcpSocketChannel>()
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        pipeline.AddLast(new ClientHandler(_messageHandler));
                    }));
            }
            catch (Exception)
            {
                _group?.ShutdownGracefullyAsync().Wait();
                _group = null;
                throw;
            }
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            IChannel channel = null;
            try
            {
                channel = AsyncHelpers.RunSync(() => _bootstrap.ConnectAsync(IPEndPoint));
                var request = new Request
                {
                    MethodName = binder.Name,
                    Arguments = args,
                    ServiceName = ServiceType.FullName
                };
                string channelId = channel.Id.AsLongText();
                _messageHandler.ReadyToWait(channelId, TimeOut, out var messageWaiter);
                var buffer = _serializer.ToBytes(request);
                channel.WriteAndFlushAsync(Unpooled.WrappedBuffer(buffer));
                _messageHandler.WaitResponse(messageWaiter);
                _messageHandler.RemoveWaiter(channelId);
                result = GetResponse(messageWaiter.ResponseString, request.MethodName);
                return true;
            }
            finally
            {
                channel?.CloseAsync().Wait();
            }
        }


        private object GetResponse(string responseString, string methodName)
        {
            Response response = _serializer.ToObject<Response>(responseString);
            if (response.Success == false)
            {
                throw new Exception(response.Message);
            }
            Type returnType = ServiceType.GetMethod(methodName).ReturnType;
            var result = returnType == typeof(void) ? null : _serializer.ToObject(response.Data, returnType);
            return result;
        }


        public Client ReplaceSerializer(ISerializer serializer)
        {
            _serializer = serializer;
            return this;
        }
    }
}
