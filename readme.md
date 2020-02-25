��ǰ���Ļ����������޸�,ǰ���Ĵ��������� : https://github.com/Coldairarrow/DotNettyRPC

����˰�װ Wjire.RPC.Server

�ͻ�����Ҫ��װ Wjire.RPC.Client

1�� ip:port ����1�� ClientGroup.
1����������Լ���ʹ���1�� Client.
1�� ClientGroup �����ж�� Client.
1�� ClientGroup ʹ��ͬһ�� ChannelPool.

һ.���������ʱ,ͨ�������ҵ���ע��[ServiceContract]���ԵĽӿ�,���� Type.FullName �� Type �ļ�ֵ��;

��.�ͻ��˷�������ǰ:

    1.���汾�ε��õķ�����Լ�� Type.
    2.����һ�� ClientWaiter ,�ڲ���װ��һ���ź��� ManualResetEventSlim ,�������˳�ʱʱ�� TimeOut,Ĭ��3��.

    �ͻ��˷�������ʱ���ݸ�����˵���Ϣʵ��:

        public class RpcRequest
        {
            public string MethodName { get; set; }
            public object[] Arguments { get; set; }
            public string ServiceContractFullName { get; set; }
        }

��.�ͻ��˷�������ʱ,ͨ�� ChannelPool ��ȡ DotNetty �� Channel,������ ClientWaiter �ȴ����ؽ��.����ʱ,���׳��쳣.

��.������յ������,����������Ϣ�е� ServiceContractFullName �ҵ���������Լ�� Type,��ͨ�� DI �����õ� service.Ȼ�����������Ϣ�е� MethodName �� Arguments,ͨ������,���� service �ķ����õ���������ؿͻ���.

    ����˷��ص���Ϣʵ��:

        public class RpcResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }
        }

��.�ͻ����յ�����˵ķ�����Ϣ��:
    1.�� Channel �Ż� ChannelPool;
    2.��������ķ�����Լ�� Type �� MethodName,ͨ������õ��÷����ķ���ֵ����,Ȼ�󽫷���˷��ص���Ϣʵ���� Data �����л���ʵ������.


ʾ��:

    ʵ��:

        //������ MessagePack ���л���Ҫ��.�����Ĭ�ϵ� Newtonsoft.Json ���л�����Ҫ��Щ����    
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

    ������Լ:
    
        //��Ҫ��װ System.ServiceModel.Primitives 4.7.0
        [ServiceContract]
        public interface ITest
        {
            Person GetPerson(int id);
        }

    ����ʵ��:
    
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


    ����������ļ�:
    
        {
          "ServerConfig": {
            "Port": 7878,
            //�������þ�ΪĬ��ֵ
            //"AcceptorEventLoopCount": 1,
            //"ClientEventLoopCount": //Ĭ�� Environment.ProcessorCount * 2;
            //"SoBacklog": 1024,
            //"SoSndbuf": 65536,
            //"SoRcvbuf": 65536
            //"MaxFrameLength":   //Ĭ�� int.MaxValue;
          }
        }
    

    ���������:

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
                          //ע�����
                          .AddSingleton<ITest, Test>()
                          .AddSingleton<IFoo, Foo>()
                          //.AddSingleton<IRpcSerializer,RpcMessagePackSerializer>()//ʹ�� MessagePack ���л�.Ĭ�ϲ��� Newtonsoft.Json
                          //�����������ַ�ʽ
                          //.AddRpcServer(x=>x.Port = 7878);
                          .AddRpcServer();//��ȡ�����ļ� "ServerConfig" �ڵ�
                  }).UseWindowsService();
        }

    
    Windows ����������

        ע��:
        
            @echo off
            title YourServiceName
            sc delete YourServiceName
            sc create YourServiceName binpath= "%~dp0%..\******.exe" displayname= "YourServiceName" depend= Tcpip start= auto
            sc description YourServiceName ��������
            pause
            exit


        ����:

            @echo off
            title YourServiceName
            net start YourServiceName
            pause
            exit

        ֹͣ:

            @echo off
            title YourServiceName
            net stop YourServiceName
            pause
            exit

        ���������ļ�����һ���������ļ���,�������ļ��з��ڷ����Ӧ�ó����Ŀ¼����.


    �ͻ���
         //�ڲ���������
         ITest client = ClientFactory.GetClient<ITest>();//Ĭ�϶�ȡ�����ļ�,���ȼ�,appsettings.Development.json > appsettings.json
         //ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 9999);
         //ITest client = ClientFactory.GetClient<ITest>(new ClientConfig{...});
         Person person = client.GetPerson(1);