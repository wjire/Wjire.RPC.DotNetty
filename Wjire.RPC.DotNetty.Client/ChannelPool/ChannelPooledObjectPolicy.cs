using System.Net;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.ObjectPool;

namespace Wjire.RPC.DotNetty.Client.ChannelPool
{
    public class ChannelPooledObjectPolicy : IPooledObjectPolicy<IChannel>
    {
        private readonly Bootstrap _bootstrap;
        private readonly IPEndPoint _ipEndPoint;

        public ChannelPooledObjectPolicy(Bootstrap bootstrap, IPEndPoint ipEndPoint)
        {
            _bootstrap = bootstrap;
            _ipEndPoint = ipEndPoint;
        }

        public IChannel Create()
        {
            return AsyncHelpers.RunSync(() => _bootstrap.ConnectAsync(_ipEndPoint));
        }

        public bool Return(IChannel obj)
        {
            return true;
        }
    }
}
