using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IServices;
using Newtonsoft.Json;
using Wjire.RPC.Client;
using Wjire.RPC.Serializer;

namespace Client
{
    internal class Program
    {
        private static readonly int num;

        private static void Main(string[] args)
        {

            Test2(12345);
            Console.ReadKey();
            Test3(11111);





            //Person p = new Person();
            //var type = p.GetType();
            //var typeJson = JsonConvert.SerializeObject(type);
            //Console.WriteLine(Encoding.UTF8.GetBytes(typeJson).Length);
            //Console.WriteLine(Encoding.UTF8.GetBytes(type.FullName).Length);
            //Console.WriteLine(typeJson);
            //Console.WriteLine(type.Name);
            //Console.WriteLine(type.FullName);
            //var t = JsonConvert.DeserializeObject<Type>(typeJson);
            //Console.WriteLine(t == type);

            {
                //ITest test1 = ClientFactory.GetClient<ITest>("127.0.0.1", 9999);
                //ITest test2 = ClientFactory.GetClient<ITest>("127.0.0.1", 9999);
                //ITest test3 = ClientFactory.GetClient<ITest>("127.0.0.1", 9999);
                //ITest test4 = ClientFactory.GetClient<ITest>("127.0.0.1", 9999);
                //IFoo foo1 = ClientFactory.GetClient<IFoo>("127.0.0.1", 9999);
                //IFoo foo2 = ClientFactory.GetClient<IFoo>("127.0.0.1", 9999);
                //IFoo foo3 = ClientFactory.GetClient<IFoo>("127.0.0.1", 9999);
                //IFoo foo4 = ClientFactory.GetClient<IFoo>("127.0.0.1", 9999);

                //List<Person> persons = new List<Person>();
                //for (int i = 0; i < 10000; i++)
                //{
                //    persons.Add(new Person { Date = DateTime.Now, Name = "wjire" + i, Id = i, Money = i });
                //}

                //int r1 = test1.GetCount(persons);
                //Person r2 = test2.GetPerson(1);
                //Console.WriteLine(r1);
                //Console.WriteLine(JsonConvert.SerializeObject(r2));
                //Console.WriteLine();
                //Person r3 = foo1.Get();
                //Console.WriteLine(JsonConvert.SerializeObject(r3));
                //Console.WriteLine(JsonConvert.SerializeObject(foo2.Get()));
                //Console.WriteLine(JsonConvert.SerializeObject(foo3.Get()));



            }

            Console.WriteLine("over");
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
            for (int i = 0; i < tasks.Length; i++)
            {
                ITest client = ClientFactory.GetClient<ITest>(() => new ClientConfig
                {
                    Ip = "127.0.0.1",
                    Port = 7878,
                    //RpcSerializer = new RpcMessagePackSerializer()
                });
                int id = i;
                tasks[i] = Task.Run(() =>
                {
                    Person person = client.GetPerson(id);
                    Console.WriteLine(JsonConvert.SerializeObject(person));
                });
            }
            Task.WaitAll(tasks);
        }

        private static void Test3(int count)
        {
            for (int i = 0; i < count; i++)
            {
                ITest client = ClientFactory.GetClient<ITest>(() => new ClientConfig
                {
                    Ip = "127.0.0.1",
                    Port = 7878,
                    //RpcSerializer = new RpcMessagePackSerializer()
                });
                Person person = client.GetPerson(i);
                Console.WriteLine(JsonConvert.SerializeObject(person));
            }
        }
    }
}
