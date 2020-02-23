using System;
using MessagePack;

namespace Wjire.RPC.DotNetty.Model
{
    [MessagePackObject]
    public class RpcRequest
    {
        [Key(0)]
        public string ServiceName { get; set; }
        [Key(1)]
        public string MethodName { get; set; }
        [Key(2)]
        public object[] Arguments { get; set; }
        [Key(3)]
        public Type ServiceType { get; set; }
    }
}
