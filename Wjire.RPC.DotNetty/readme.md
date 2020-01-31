在前辈的基础上做的修改和优化,前辈的代码在这里 : https://github.com/Coldairarrow/DotNettyRPC
    
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
    1.服务端需要引用服务契约,服务实现,实体.比如示例中的 ITest,Test,Person 所对应的类库;
    2.客户端需要引用服务契约,实体;
    3.客户端链接已做单例,长连接,对象池.
    4.默认序列化方式为 Json,内置了另外一种: MessagePack


本机测试:
    1.Task.Run() 10000 次,平均每次耗时 0.12 ms;
    2.单线程运行 10000 次,平均每次耗时 0.3 ms.
