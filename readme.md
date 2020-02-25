在前辈的基础上做的修改,前辈的代码在这里 : https://github.com/Coldairarrow/DotNettyRPC

服务端安装 Wjire.RPC.Server

客户端需要安装 Wjire.RPC.Client

1个 ip:port 代表1个 ClientGroup.
1个服务器契约类型代表1个 Client.
1个 ClientGroup 可以有多个 Client.
1个 ClientGroup 使用同一个 ChannelPool.

一.服务端启动时,通过反射找到标注有[ServiceContract]特性的接口,构建 Type.FullName 和 Type 的键值对;

二.客户端发起请求前:

    1.保存本次调用的服务契约的 Type.
    2.构造一个 ClientWaiter ,内部封装了一个信号量 ManualResetEventSlim ,并设置了超时时间 TimeOut,默认3秒.

    客户端发起请求时传递给服务端的消息实体:

        public class RpcRequest
        {
            public string MethodName { get; set; }
            public object[] Arguments { get; set; }
            public string ServiceContractFullName { get; set; }
        }

三.客户端发起请求时,通过 ChannelPool 获取 DotNetty 的 Channel,并调用 ClientWaiter 等待返回结果.若超时,则抛出异常.

四.服务端收到请求后,根据请求消息中的 ServiceContractFullName 找到服务器契约的 Type,再通过 DI 容器得到 service.然后根据请求消息中的 MethodName 及 Arguments,通过反射,调用 service 的方法得到结果并返回客户端.

    服务端返回的消息实体:

        public class RpcResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }
        }

五.客户端收到服务端的返回消息后:
    1.将 Channel 放回 ChannelPool;
    2.根据请求的服务契约的 Type 及 MethodName,通过反射得到该方法的返回值类型,然后将服务端返回的消息实体中 Data 反序列化成实际类型.


示例:

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
                          .AddRpcServer();//读取配置文件 "ServerConfig" 节点
                  }).UseWindowsService();
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
         //ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 9999);
         //ITest client = ClientFactory.GetClient<ITest>(new ClientConfig{...});
         Person person = client.GetPerson(1);