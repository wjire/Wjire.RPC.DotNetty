using System;
using DotNetty.Buffers;

namespace Wjire.RPC.DotNetty.Client
{
    public interface ISerializer
    {
        object ToObject(object obj, Type type);

        byte[] ToBytes(object obj);

        T ToObject<T>(IByteBuffer byteBuffer);
    }
}
