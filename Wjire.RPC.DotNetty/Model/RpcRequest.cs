using System;
using MessagePack;

namespace Wjire.RPC.DotNetty.Model
{
    [MessagePackObject]
    public class RpcRequest
    {
        [Key(0)]
        public string MethodName { get; set; }

        [Key(1)]
        public object[] Arguments { get; set; }

        [Key(2)]
        public Type ServiceType { get; set; }

        //传类型名称,操作起来不适很优雅,改用直接传类型
        //[Key(3)]
        //public string ServiceName { get; set; }
    }
}
