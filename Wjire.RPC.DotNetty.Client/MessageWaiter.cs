using System;
using System.Threading;

namespace Wjire.RPC.DotNetty.Client
{
    internal class MessageWaiter : IDisposable
    {
        internal string ResponseString { get; set; }

        private readonly ManualResetEventSlim _mutex = new ManualResetEventSlim();
        
        internal void WaitResponse(TimeSpan timeOut)
        {
            _mutex.Wait(timeOut);
        }

        internal void SetResponseCompleted(string responseString)
        {
            ResponseString = responseString;
            _mutex.Set();
        }

        public void Dispose()
        {
            _mutex?.Dispose();
        }
    }
}
