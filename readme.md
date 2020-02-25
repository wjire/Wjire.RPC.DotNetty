在前辈的基础上做的修改,前辈的代码在这里 : https://github.com/Coldairarrow/DotNettyRPC

服务端安装 Wjire.RPC.Server

客户端安装 Wjire.RPC.Client

example:

实体:

    //特性是 MessagePack 序列化需要的.如果用默认的 Newtonsoft.Json 序列化则不需要这些特性    
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

    //需要安装 System.ServiceModel.Primitives 4.7.0
    [ServiceContract]
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
            };
        }
    }


服务端配置文件:

    {
      "ServerConfig": {
        "Port": 7878,
        //以下配置均为默认值
        //"AcceptorEventLoopCount": 1,
        //"ClientEventLoopCount": //默认 Environment.ProcessorCount * 2;
        //"SoBacklog": 1024,
        //"SoSndbuf": 65536,
        //"SoRcvbuf": 65536
        //"MaxFrameLength":   //默认 int.MaxValue;
      }
    }


服务端宿主:

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
                      //注册服务
                      .AddSingleton<ITest, Test>()
                      .AddSingleton<IFoo, Foo>()
                      //.AddSingleton<IRpcSerializer,RpcMessagePackSerializer>()//使用 MessagePack 序列化.默认采用 Newtonsoft.Json
                      //启动服务两种方式
                      //.AddRpcServer(x=>x.Port = 7878);
                      .AddRpcServer();//读取配置文件
              }).UseWindowsService();//可做成windows服务
    }


Windows 服务常用命令

    注册:
    
        @echo off
        title YourServiceName
        sc delete YourServiceName
        sc create YourServiceName binpath= "%~dp0%..\******.exe" displayname= "YourServiceName" depend= Tcpip start= auto
        sc description YourServiceName 服务描述
        pause
        exit


    启动:

        @echo off
        title YourServiceName
        net start YourServiceName
        pause
        exit

    停止:

        @echo off
        title YourServiceName
        net stop YourServiceName
        pause
        exit

    上述三个文件放在一个单独的文件夹,并将该文件夹放在服务端应用程序根目录即可.


客户端

     //内部已做单例
     ITest client = ClientFactory.GetClient<ITest>();//默认读取配置文件,优先级,appsettings.Development.json > appsettings.json
     //ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 7878);
     //ITest client = ClientFactory.GetClient<ITest>(new ClientConfig{...});
     Person person = client.GetPerson(1);

客户端配置:
    
    {
      "ClientConfig": {
        "Ip": "127.0.0.1",
        "Port": 7878
        //以下配置均为默认值
        //"TimeOut": 3, //单位:秒
        //"PooledObjectMax": //默认 Environment.ProcessorCount * 2;
        //"SoSndbuf": 65536,
        //"SoRcvbuf": 65536
      }
    }