��ǰ���Ļ����������޸�,ǰ���Ĵ��������� : https://github.com/Coldairarrow/DotNettyRPC

����˰�װ Wjire.RPC.Server

�ͻ��˰�װ Wjire.RPC.Client

example:

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
                      .AddRpcServer();//��ȡ�����ļ�
              }).UseWindowsService();//������windows����
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
     //ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 7878);
     //ITest client = ClientFactory.GetClient<ITest>(new ClientConfig{...});
     Person person = client.GetPerson(1);

�ͻ�������:
    
    {
      "ClientConfig": {
        "Ip": "127.0.0.1",
        "Port": 7878
        //�������þ�ΪĬ��ֵ
        //"TimeOut": 3, //��λ:��
        //"PooledObjectMax": //Ĭ�� Environment.ProcessorCount * 2;
        //"SoSndbuf": 65536,
        //"SoRcvbuf": 65536
      }
    }