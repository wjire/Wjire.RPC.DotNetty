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
            string json = JsonConvert.SerializeObject(obj);
            return ToObject(json, type);
        }

        public byte[] ToBytes(object obj)
        {
            string json = ToString(obj);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
