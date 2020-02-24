��ǰ���Ļ����������޸ĺ��Ż�,ǰ���Ĵ��������� : https://github.com/Coldairarrow/DotNettyRPC

��    ��:   .NET Standard 2.0

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


һ.���������ʱ:

    1.ͨ�� IConfiguration ���������ļ�;
    2.ͨ�������ҵ���ע��[ServiceContract]���ԵĽӿ�,���� Type.FullName �� Type �ļ�ֵ��;

��.�ͻ��˷�������ǰ:

    1.���汾�ε��õķ�����Լ�� Type.
    2.����һ�� ClientWaiter ,�ڲ���װ��һ���ź��� ManualResetEventSlim ,�������˳�ʱʱ�� TimeOut.

    �ͻ��˷�������ʱ���ݸ�����˵���Ϣʵ��:

        public class RpcRequest
        {
            public string MethodName { get; set; }
            public object[] Arguments { get; set; }
            public string ServiceContractFullName { get; set; }
        }

��.�ͻ��˷�������:
    ͨ�����ӳػ�ȡ DotNetty �� Channel,�������󲢵��� ClientWaiter �ȴ����ؽ��.����ʱ,���׳��쳣.

��.������յ������,����������Ϣ�е� ServiceContractFullName �ҵ���������Լ�� Type,��ͨ��΢���Դ���DI�����õ� service.Ȼ�����������Ϣ�е� MethodName �� Arguments,ͨ������,���� service �ķ����õ���������ؿͻ���.

    ����˷��ص���Ϣʵ��:

        public class RpcResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }
        }

��.�ͻ����յ�����˵ķ�����Ϣ��:
    1.�� Channel �Ż����ӳ�;
    2.������Ϣʵ���е� Data ������� MethodName,����ķ�����Լ�� Type,ͨ�����佫 Data ת����ʵ������.


ʾ��:

    ʵ��:

        //������ MessagePack ���л���Ҫ��.�����Ĭ�ϵ� Json ���л�����Ҫ��Щ����    
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
            "Port": 9999,
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
                            //.AddSingleton<IRpcSerializer, RpcJsonSerializer>()//Ĭ�Ͼ��� Json
                            //.AddSingleton<IRpcSerializer, RpcMessagePackSerializer>();// MessagePack
                              .AddHostedService<Wjire.RPC.DotNetty.Server>();
                      }).UseWindowsService();//������ windows ����
            }
        }

    �����ÿ���̨,Ҳ��������windows����.

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

    �ͻ���
         
         ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 9999);//�ڲ���������
         Person person = client.GetPerson(1);//�ڲ�ʵ��Ϊ������+�����.