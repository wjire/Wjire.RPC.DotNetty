using DotNetty.Transport.Channels;

namespace Wjire.RPC.DotNetty.Extension
{
    public static class ChannelExtension
    {
        internal static string GetId(this IChannel channel)
        {
            return channel.Id.AsLongText();
        }
    }
}
