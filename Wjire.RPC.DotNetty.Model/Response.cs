
namespace Wjire.RPC.DotNetty.Model
{
    public class Response
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }
    }
}
