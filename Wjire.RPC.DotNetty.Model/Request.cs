namespace Wjire.RPC.DotNetty.Model
{
    public class Request
    {
        public string ServiceName { get; set; }

        public string MethodName { get; set; }

        public object[] Arguments { get; set; }
    }
}
