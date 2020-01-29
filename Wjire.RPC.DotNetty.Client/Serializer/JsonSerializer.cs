using System;
using System.Text;
using DotNetty.Buffers;
using Newtonsoft.Json;

namespace Wjire.RPC.DotNetty.Client
{
    public class JsonSerializer : ISerializer
    {
        public object ToObject(object obj, Type type)
        {
            string json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject(json, type);
        }

        public byte[] ToBytes(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(json);
        }


        public T ToObject<T>(IByteBuffer byteBuffer)
        {
            string json = byteBuffer.ToString(Encoding.UTF8);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public T ToObject<T>(byte[] bytes)
        {
            string json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
