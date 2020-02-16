using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IServices;
using Newtonsoft.Json;
using Wjire.RPC.DotNetty.Client;

namespace Client
{
    internal class Program
    {
        private static readonly int num;

        private static void Main(string[] args)
        {
            //Test(10000);
            List<Person> persons = new List<Person>();
            for (int i = 0; i < 10000; i++)
            {
                persons.Add(new Person { Date = DateTime.Now, Name = "wjire" + i, Id = i, Money = i });
            }
            string json = JsonConvert.SerializeObject(persons);
            Console.WriteLine(Encoding.UTF8.GetBytes(json).Length);
            //ITest test = ClientFactory.GetClient<ITest>("127.0.0.1", 7878);
            //IFoo foo = ClientFactory.GetClient<IFoo>("127.0.0.1", 7878);
            //Person testResult = test.GetPerson(1);
            //Console.WriteLine(JsonConvert.SerializeObject(testResult));
            //Console.WriteLine(JsonConvert.SerializeObject(foo.Get()));


            //var client = ClientFactory.GetClient<ITest>("139.224.208.128", 7878);
            ITest client = ClientFactory.GetClient<ITest>(new ClientConfig("139.224.208.128", 7878)
            {
                TimeOut = TimeSpan.FromSeconds(300)
            });
            //client.GetPerson(1);
            int count = client.GetCount(persons);
            Console.WriteLine(count);

            {

                //int count = 10000;
                //for (int i = 0; i < 50; i++)
                //{
                //    Stopwatch sw = new Stopwatch();
                //    sw.Start();
                //    Test2(count);
                //    sw.Stop();
                //    Console.WriteLine($"运行{count}次,共耗时:{sw.ElapsedMilliseconds} ms ,每次耗时:" + (double)sw.ElapsedMilliseconds / count + " ms");
                //}
                //Console.WriteLine("over");
            }

            Console.ReadKey();
        }


        private static void Test(int count)
        {
            List<Person> persons = new List<Person>();
            for (int i = 0; i < 10000; i++)
            {
                persons.Add(new Person { Date = DateTime.Now, Name = "wjire" + i, Id = i, Money = i });
            }

            string json = JsonConvert.SerializeObject(persons);
            Console.WriteLine(Encoding.UTF8.GetBytes(json).Length);
        }


        private static void Test2(int count)
        {
            Task[] tasks = new Task[count];
            ClientConfig config = new ClientConfig("127.0.0.1", 7878)
            {
                AllIdleTimeSeconds = 10
            };
            ITest client = ClientFactory.GetClient<ITest>(config);
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    Person person = client.GetPerson(1);
                });
            }
            Task.WaitAll(tasks);
        }

        private static void Test3(int count)
        {
            ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 7878);
            for (int i = 0; i < count; i++)
            {
                Person person = client.GetPerson(i);
            }
        }
    }
}
