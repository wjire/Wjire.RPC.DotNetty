using System;
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

            Test2(11111);
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
            ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 7878);
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    //Console.Write(client.GetPerson(Interlocked.Increment(ref num)).Id + ",");
                    Person person = client.GetPerson(Interlocked.Increment(ref num));
                    Console.WriteLine(JsonConvert.SerializeObject(person));
                });
                //Thread.Sleep(100);
            }

            Task.WaitAll(tasks);
        }
    }
}
