using System.Net;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.ObjectPool;
using Wjire.RPC.Client.Helper;

namespace Wjire.RPC.Client
{
    internal class ChannelPooledObjectPolicy : IPooledObjectPolicy<IChannel>
    {
        private readonly Bootstrap _bootstrap;
        private readonly IPEndPoint _remoteAddress;

        internal ChannelPooledObjectPolicy(Bootstrap bootstrap, IPEndPoint remoteAddress)
        {
            _bootstrap = bootstrap;
            _remoteAddress = remoteAddress;
        }

        public IChannel Create()
        {
            //return AsyncHelper1.RunSync(() => _bootstrap.ConnectAsync(_remoteAddress));
            return AsyncHelper2.RunSync(() => _bootstrap.ConnectAsync(_remoteAddress));
        }

        public bool Return(IChannel obj)
        {
            return true;
        }
    }
}
