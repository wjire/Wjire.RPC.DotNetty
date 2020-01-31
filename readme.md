在前辈的基础上做的修改和优化,前辈的代码在这里 : https://github.com/Coldairarrow/DotNettyRPC

框    架:   .NET Standard 2.0
通    信:   DotNetty
动态代理:   ImpromptuInterface
序 列 化:   Newtonsort.Json,MessagePack
连 接 池:   Microsoft.Extensions.ObjectPool
依赖注入:   Microsoft.Extensions.DependencyInjection
日志记录:   Wjire.Log (自制)    


示例:

实体:

    //特性是 MessagePack 序列化需要的.如果用默认的 Json 序列化则不需要这些特性    
    [MessagePackObject]
    public class Person
    {
        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public decimal Money { get; set; }
        [Key(3)]
        public DateTime Date { get; set; }
    }

服务契约:

    public interface ITest
    {
        Person GetPerson(int id);
    }

服务实现:
    
    public class Test : ITest
    {
        public Person GetPerson(int id)
        {
            return new Person
            {
                Id = id,
                Name = Guid.NewGuid().ToString(),
                Date = DateTime.Now,
                Money = 12000M,
            };
        }
    }


服务端
     Server server = new Server(7878);
     
     //.net core 自带的DI容器
     ServiceCollection services = new ServiceCollection();
     services.AddSingleton<ITest, Test>();

     server.RegisterServices(services);
     server.Start().Wait();

客户端
         
     ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 7878);//内部已做单例
     Person person = client.GetPerson(1);//内部实现为长连接+对象池.

本机测试:
    1.Task.Run() 10000 次,平均每次耗时 0.12 ms;
    2.单线程运行 10000 次,平均每次耗时 0.28 ms.
