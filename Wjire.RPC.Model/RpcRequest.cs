using MessagePack;

namespace Wjire.RPC.Model
{
    [MessagePackObject]
    public class RpcRequest
    {
        [Key(0)]
        public string MethodName { get; set; }

        [Key(1)]
        public object[] Arguments { get; set; }

        [Key(2)]
        public string ServiceContractFullName { get; set; }
    }
}
