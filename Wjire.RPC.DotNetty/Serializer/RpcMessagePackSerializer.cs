using System;
using MessagePack;

namespace Wjire.RPC.DotNetty.Serializer
{
    public class RpcMessagePackSerializer : IRpcSerializer
    {
        public object ToObject(object obj, Type type)
        {
            return MessagePackSerializer.Deserialize(type, MessagePackSerializer.Serialize(obj));
        }

        public byte[] ToBytes(object obj)
        {
            return MessagePackSerializer.Serialize(obj);
        }

        public T ToObject<T>(byte[] bytes)
        {
            return MessagePackSerializer.Deserialize<T>(bytes);
        }
    }
}
