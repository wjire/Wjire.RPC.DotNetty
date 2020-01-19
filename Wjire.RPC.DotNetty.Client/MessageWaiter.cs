using System;
using System.Threading;

namespace Wjire.RPC.DotNetty.Client
{
    internal class MessageWaiter : IDisposable
    {
        internal string ResponseString { get; set; }

        private readonly ManualResetEventSlim _mutex = new ManualResetEventSlim();

        private readonly TimeSpan _timeOut;

        public MessageWaiter(TimeSpan timeOut)
        {
            _timeOut = timeOut;
        }

        internal void WaitResponse()
        {
            _mutex.Wait(_timeOut);
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
