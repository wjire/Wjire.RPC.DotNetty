using System;
using System.Text;
using Newtonsoft.Json;

namespace Wjire.RPC.DotNetty.Serializer
{
    public class JsonSerializer : ISerializer
    {
        public string ToString(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T ToObject<T>(string objString)
        {
            return JsonConvert.DeserializeObject<T>(objString);
        }

        public object ToObject(string objString, Type type)
        {
            return JsonConvert.DeserializeObject(objString, type);
        }

        public object ToObject(object obj, Type type)
        {
            var json = JsonConvert.SerializeObject(obj);
            return ToObject(json, type);
        }

        public byte[] ToBytes(object obj)
        {
            var json = ToString(obj);
            return Encoding.UTF8.GetBytes(json);
        }

        public T ToObject<T>(byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes);
            return ToObject<T>(json);
        }

        public object ToObject(byte[] bytes, Type type)
        {
            var json = Encoding.UTF8.GetString(bytes);
            return ToObject(json, type);
        }
    }
}
