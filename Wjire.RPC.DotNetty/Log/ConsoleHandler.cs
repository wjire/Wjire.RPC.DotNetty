using System;

namespace Wjire.RPC.DotNetty.Log
{
    internal class ConsoleHandler : ILogHandler
    {
        public void WriteLog(Exception ex, string remark, object request = null, object response = null, string path = null)
        {
            Console.WriteLine(remark + ex);
        }

        public void WriteLog(string content, string path = null)
        {
            Console.WriteLine(content);
        }
    }
}
