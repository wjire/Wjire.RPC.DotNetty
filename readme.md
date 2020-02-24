在前辈的基础上做的修改和优化,前辈的代码在这里 : https://github.com/Coldairarrow/DotNettyRPC

框    架:   .NET Standard 2.0

    "DotNetty.Codecs" Version="0.6.0"
    "DotNetty.Transport" Version="0.6.0"
    "ImpromptuInterface" Version="7.0.1"
    "MessagePack" Version="2.1.90"
    "Microsoft.Extensions.Hosting" Version="3.1.2"
    "Microsoft.Extensions.Hosting.Abstractions" Version="3.1.2"
    "Microsoft.Extensions.Hosting.WindowsServices" Version="3.1.2"
    "Microsoft.Extensions.Configuration" Version="3.1.2"
    "Microsoft.Extensions.Configuration.Binder" Version="3.1.2"
    "Microsoft.Extensions.DependencyInjection" Version="3.1.2"
    "Microsoft.Extensions.Hosting.Abstractions" Version="3.1.2"
    "Microsoft.Extensions.ObjectPool" Version="3.1.2"
    "Newtonsoft.Json" Version="12.0.3"
    "System.ServiceModel.Primitives" Version="4.7.0"
    "Wjire.Log" Version="1.0.3"


一.服务端启动时:

    1.通过 IConfiguration 加载配置文件;
    2.通过反射找到标注有[ServiceContract]特性的接口,构建 Type.FullName 和 Type 的键值对;

二.客户端发起请求前:

    1.保存本次调用的服务契约的 Type.
    2.构造一个 ClientWaiter ,内部封装了一个信号量 ManualResetEventSlim ,并设置了超时时间 TimeOut.

    客户端发起请求时传递给服务端的消息实体:

        public class RpcRequest
        {
            public string MethodName { get; set; }
            public object[] Arguments { get; set; }
            public string ServiceContractFullName { get; set; }
        }

三.客户端发起请求:
    通过连接池获取 DotNetty 的 Channel,发起请求并调用 ClientWaiter 等待返回结果.若超时,则抛出异常.

四.服务端收到请求后,根据请求消息中的 ServiceContractFullName 找到服务器契约的 Type,再通过微软自带的DI容器得到 service.然后根据请求消息中的 MethodName 及 Arguments,通过反射,调用 service 的方法得到结果并返回客户端.

    服务端返回的消息实体:

        public class RpcResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }
        }

五.客户端收到服务端的返回消息后:
    1.将 Channel 放回连接池;
    2.根据消息实体中的 Data 及请求的 MethodName,保存的服务契约的 Type,通过反射将 Data 转换成实际类型.


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
            "Port": 9999,
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

    可以用控制台,也可以做成windows服务.

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

    客户端
         
         ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 9999);//内部已做单例
         Person person = client.GetPerson(1);//内部实现为长连接+对象池.