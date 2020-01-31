using System;

namespace Wjire.RPC.DotNetty.Log
{
    public interface ILogHandler
    {
        void WriteLog(Exception ex, string remark, object request = null, object response = null, string path = null);

        void WriteLog(string content, string path = null);
    }
}
