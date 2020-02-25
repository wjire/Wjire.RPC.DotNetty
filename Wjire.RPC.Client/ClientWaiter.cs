using System;
using System.Threading;

namespace Wjire.RPC.Client
{
    internal class ClientWaiter : IDisposable
    {
        private readonly TimeSpan _timeOut;
        internal byte[] Bytes { get; set; }

        private readonly ManualResetEventSlim _mutex = new ManualResetEventSlim();


        internal ClientWaiter(TimeSpan timeOut)
        {
            _timeOut = timeOut;
        }

        internal void Wait()
        {
            _mutex.Wait(new CancellationTokenSource(_timeOut).Token);
        }

        internal void Set(byte[] bytes)
        {
            Bytes = bytes;
            _mutex.Set();
        }

        public void Dispose()
        {
            _mutex?.Dispose();
        }
    }
}
