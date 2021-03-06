﻿
using MessagePack;

namespace Wjire.RPC.Common.Message
{
    [MessagePackObject]
    public class RpcResponse
    {
        [Key(0)]
        public bool Success { get; set; }

        [Key(1)]
        public string Message { get; set; }

        [Key(2)]
        public object Data { get; set; }
    }
}
