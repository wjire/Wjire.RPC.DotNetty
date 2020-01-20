using System;
using System.Threading;
using System.Threading.Tasks;
using IServices;
using Newtonsoft.Json;
using Wjire.RPC.DotNetty.Client;

namespace Client
{
    class Program
    {
        private static int num;

        static void Main(string[] args)
        {
            Test2(11112);
            Console.WriteLine("over");
            Console.ReadKey();
        }

        private static void Test(int count)
        {
            Thread[] threads = new Thread[count];
            for (int i = 0; i < count; i++)
            {
                threads[i] = new Thread(() =>
                    {
                        ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 7878);
                        //Console.WriteLine(Thread.CurrentThread.ManagedThreadId + ":" + JsonConvert.SerializeObject(client.GetPerson(Interlocked.Increment(ref num)))+"\r\n");
                        var person = client.GetPerson(Interlocked.Increment(ref num));
                        Console.WriteLine(Thread.CurrentThread.ManagedThreadId + ":" + JsonConvert.SerializeObject(person));
                    })
                { IsBackground = true };
            }

            foreach (Thread t in threads)
            {
                t.Start();
            }
        }


        private static void Test2(int count)
        {
            Task[] tasks = new Task[count];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 7878);
                    //Console.Write(client.GetPerson(Interlocked.Increment(ref num)).Id + ",");
                    var person = client.GetPerson(Interlocked.Increment(ref num));
                    Console.WriteLine(JsonConvert.SerializeObject(person));
                });
            }

            Task.WaitAll(tasks);
        }
    }
}
