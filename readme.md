在前辈的基础上做的修改和优化,前辈的代码在这里 : https://github.com/Coldairarrow/DotNettyRPC

框    架:   .NET Standard 2.0

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

    internal class Program
    {
        private static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                  .ConfigureServices((hostContext, services) =>
                  {
                      services
                          .AddSingleton<ITest, Test>()
                        //.AddSingleton<IRpcSerializer, RpcJsonSerializer>()//默认就是 Json
                        //.AddSingleton<IRpcSerializer, RpcMessagePackSerializer>();// MessagePack
                          .AddHostedService<Wjire.RPC.DotNetty.Server>();
                  }).UseWindowsService();//方便做 windows 服务
        }
    }


客户端
         
     ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 7878);//内部已做单例
     Person person = client.GetPerson(1);//内部实现为长连接+对象池.


可以用控制台,也可以做成windows服务.

Windows 服务常用命令

    注册:
        
        @echo off
        title ServiceName
        sc delete ServiceName
        sc create ServiceName binpath= "%~dp0%..\******.exe" displayname= "ServiceName" depend= Tcpip start= auto
        sc description ServiceName 服务描述
        pause
        exit


    启动:

        @echo off
        title ServiceName
        net start ServiceName
        pause
        exit

    停止:

        @echo off
        title ServiceName
        net stop ServiceName
        pause
        exit