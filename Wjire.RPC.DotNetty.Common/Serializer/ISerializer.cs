using System;

namespace Wjire.RPC.DotNetty.Serializer
{
    public interface ISerializer
    {
        string ToString(object obj);

        object ToObject(byte[] bytes, Type type);

        object ToObject(string objString, Type type);

        object ToObject(object obj, Type type);

        T ToObject<T>(byte[] bytes);

        T ToObject<T>(string objString);
      
        byte[] ToBytes(object obj);
    }
}
