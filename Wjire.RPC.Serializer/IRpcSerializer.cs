using System;

namespace Wjire.RPC.Serializer
{
    public interface IRpcSerializer
    {
        object ToObject(object obj, Type type);

        byte[] ToBytes(object obj);

        T ToObject<T>(byte[] bytes);
    }
}
