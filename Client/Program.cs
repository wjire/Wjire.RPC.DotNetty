using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using IServices;
using Newtonsoft.Json;
using Wjire.RPC.DotNetty.Client;

namespace Client
{
    internal class Program
    {
        private static int num;

        private static void Main(string[] args)
        {
            //ITest test = ClientFactory.GetClient<ITest>("127.0.0.1", 7878);
            //IFoo foo = ClientFactory.GetClient<IFoo>("127.0.0.1", 7878);
            //Person testResult = test.GetPerson(1);
            //Console.WriteLine(JsonConvert.SerializeObject(testResult));
            //Console.WriteLine(JsonConvert.SerializeObject(foo.Get()));

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Test2(1);
            sw.Stop();
            Console.WriteLine("耗时" + sw.ElapsedMilliseconds);
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
                        Person person = client.GetPerson(Interlocked.Increment(ref num));
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
            //var config = new ClientConfig("139.224.208.128", 7878)
            var config = new ClientConfig("127.0.0.1", 7878)
            {
                AllIdleTimeSeconds = 10
            };
            ITest client = ClientFactory.GetClient<ITest>(config);
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    Person person = client.GetPerson(Interlocked.Increment(ref num));
                    Console.WriteLine(JsonConvert.SerializeObject(person));
                });
            }
            Task.WaitAll(tasks);
        }


        private static void Test3(int count)
        {
            ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 7878);
            for (int i = 0; i < count; i++)
            {
                var id = Interlocked.Increment(ref num);
                Person person = client.GetPerson(id);
                Console.WriteLine(id + ":" + JsonConvert.SerializeObject(person));
            }
        }

        private static void Test4(int count)
        {
            ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 7878);
            for (int i = 0; i < count; i++)
            {
                var res = client.Get("wjire");
                Console.WriteLine(i + ":" + res);
            }
        }
    }
}
