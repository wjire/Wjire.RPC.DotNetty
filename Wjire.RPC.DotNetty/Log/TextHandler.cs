using System;
using Wjire.Log;

namespace Wjire.RPC.DotNetty.Log
{
    internal class TextHandler : ILogHandler
    {

        public void WriteLog(Exception ex, string remark, object request = null, object response = null, string path = null)
        {
            LogService.WriteException(ex, remark, request, response, path);
        }

        public void WriteLog(string content, string path = null)
        {
            LogService.WriteText(content, path);
        }
    }
}
