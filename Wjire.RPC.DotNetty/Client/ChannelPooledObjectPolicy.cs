using System.Net;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.ObjectPool;
using Wjire.RPC.DotNetty.Helper;

namespace Wjire.RPC.DotNetty.Client.ChannelPool
{
    public class ChannelPooledObjectPolicy : IPooledObjectPolicy<IChannel>
    {
        private readonly Bootstrap _bootstrap;
        private readonly IPEndPoint _remoteAddress;

        public ChannelPooledObjectPolicy(Bootstrap bootstrap, IPEndPoint remoteAddress)
        {
            _bootstrap = bootstrap;
            _remoteAddress = remoteAddress;
        }

        public IChannel Create()
        {
            return AsyncHelpers.RunSync(() => _bootstrap.ConnectAsync(_remoteAddress));
        }

        public bool Return(IChannel obj)
        {
            return true;
        }
    }
}
