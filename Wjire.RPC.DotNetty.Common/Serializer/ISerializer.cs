using System;

namespace Wjire.RPC.DotNetty.Serializer
{
    public interface ISerializer
    {
        string ToString(object obj);

        T ToObject<T>(string objString);

        object ToObject(string objString, Type type);

        object ToObject(object obj, Type type);

        byte[] ToBytes(object obj);

        T ToObject<T>(byte[] bytes);

        object ToObject(byte[] bytes, Type type);
    }
}
