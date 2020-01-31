在前辈的基础上做的修改和优化,前辈的代码在这里 : https://github.com/Coldairarrow/DotNettyRPC

框    架:   .NET Standard 2.0
通    信:   DotNetty.Handlers,DotNetty.Transport
动态代理:   ImpromptuInterface
序 列 化:   Newtonsort.Json,MessagePack
连 接 池:   Microsoft.Extensions.ObjectPool
依赖注入:   Microsoft.Extensions.DependencyInjection
日志记录:   Wjire.Log (自制)    


示例:
服务端
     Server server = new Server(7878);
     
     //.net core 自带的DI容器
     ServiceCollection services = new ServiceCollection();
     services.AddSingleton<ITest, Test>();

     server.RegisterServices(services);
     server.Start().Wait();

客户端
         
     ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 7878);
     Person person = client.GetPerson(1);

PS:
    1.客户端链接已做单例,长连接,对象池.
    2.默认序列化方式为 Json,内置了另外一种: MessagePack


本机测试:
    1.Task.Run() 10000 次,平均每次耗时 0.12 ms;
    2.单线程运行 10000 次,平均每次耗时 0.3 ms.
