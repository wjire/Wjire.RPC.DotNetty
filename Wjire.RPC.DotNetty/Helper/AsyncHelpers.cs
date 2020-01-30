using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wjire.RPC.DotNetty.Helper
{
    /// <summary>
    /// https://stackoverflow.com/questions/5095183/how-would-i-run-an-async-taskt-method-synchronously
    /// </summary>
    internal static class AsyncHelpers
    {

        /// <summary>
        /// Execute's an async Task<T> method which has a void return value synchronously
        /// </summary>
        /// <param name="task">Task<T> method to execute</param>
        public static void RunSync(Func<Task> task)
        {
            SynchronizationContext oldContext = SynchronizationContext.Current;
            ExclusiveSynchronizationContext synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            synch.Post(async _ =>
            {
                try
                {
                    await task();
                }
                catch (Exception e)
                {
                    synch.InnerException = e;
                    throw;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();
            SynchronizationContext.SetSynchronizationContext(oldContext);
        }


        /// <summary>
        /// Execute's an async Task<T> method which has a T return type synchronously
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="task">Task<T> method to execute</param>
        /// <returns></returns>
        public static T RunSync<T>(Func<Task<T>> task)
        {
            SynchronizationContext oldContext = SynchronizationContext.Current;
            ExclusiveSynchronizationContext synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            T result = default;
            synch.Post(async _ =>
            {
                try
                {
                    result = await task();
                }
                catch (Exception e)
                {
                    synch.InnerException = e;
                    throw;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();
            SynchronizationContext.SetSynchronizationContext(oldContext);
            return result;
        }

        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            private bool _done;
            private readonly AutoResetEvent _workItemsWaiting = new AutoResetEvent(false);
            private readonly Queue<(SendOrPostCallback, object)> _queue = new Queue<(SendOrPostCallback, object)>();
            internal Exception InnerException { get; set; }

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                _queue.Enqueue((d, state));
                _workItemsWaiting.Set();
            }

            public void EndMessageLoop()
            {
                Post(_ => _done = true, null);
            }

            internal void BeginMessageLoop()
            {
                while (_done == false)
                {
                    if (_queue.Count > 0)
                    {
                        (SendOrPostCallback, object) t = _queue.Dequeue();
                        t.Item1(t.Item2);
                        if (InnerException != null) // the method threw an exeption
                        {
                            throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
                        }
                    }
                    else
                    {
                        _workItemsWaiting.WaitOne();
                    }
                }
            }
        }
    }
}
