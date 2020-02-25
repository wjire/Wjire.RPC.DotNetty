using System;
using Microsoft.Extensions.Logging;

namespace Wjire.RPC.Server
{
    internal static class LoggerExtension
    {
        internal static void ClosingServerException(this ILogger logger, Exception ex)
        {
            logger.LogError(ex, "Throw an exception when closing server");
        }

        internal static void ServerHandlerCaughtException(this ILogger logger, Exception ex)
        {
            logger.LogError(ex, "ServerHandler throw the exception");
        }
    }
}
