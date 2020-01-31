using System;
using Wjire.RPC.DotNetty.Log;

namespace Wjire.RPC.DotNetty
{
    public static class RpcLogService
    {
        private static ILogHandler _logHandler = RpcConfig.DefaultLogHandler;

        public static void WriteLog(Exception ex, string remark, object request = null, object response = null, string path = null)
        {
            _logHandler.WriteLog(ex, remark, request, response, path);
        }

        public static void WriteLog(string content, string path = null)
        {
            _logHandler.WriteLog(content, path);
        }

        public static void UseConsoleLog()
        {
            _logHandler = new ConsoleHandler();
        }

        public static void UseTextLog()
        {
            _logHandler = new TextHandler();
        }
    }
}
