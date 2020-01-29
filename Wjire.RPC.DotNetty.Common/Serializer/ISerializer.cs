using System;

namespace Wjire.RPC.DotNetty.Common
{
    public interface ISerializer
    {
        object ToObject(object obj, Type type);

        byte[] ToBytes(object obj);

        T ToObject<T>(byte[] bytes);
    }
}
