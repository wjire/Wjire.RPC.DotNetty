��ǰ���Ļ����������޸ĺ��Ż�,ǰ���Ĵ��������� : https://github.com/Coldairarrow/DotNettyRPC

��    ��:   .NET Standard 2.0

ͨ    ��:   DotNetty

��̬����:   ImpromptuInterface

�� �� ��:   Newtonsort.Json,MessagePack

�� �� ��:   Microsoft.Extensions.ObjectPool

����ע��:   Microsoft.Extensions.DependencyInjection

��־��¼:   Wjire.Log


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
                Money = 12000M,
            };
        }
    }


�����

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


�ͻ���
         
     ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 7878);//�ڲ���������
     Person person = client.GetPerson(1);//�ڲ�ʵ��Ϊ������+�����.


Windows ����������

    ע��:
        
        @echo off
        title 0MyService
        sc delete 0MyService
        sc create 0MyService binpath= "%~dp0%..\Server.exe" displayname= "0MyService" depend= Tcpip start= auto
        sc description 0MyService ��������
        pause
        exit


    ����:

        @echo off
        title 0MyService
        net start 0MyService
        pause
        exit

    ֹͣ:

        @echo off
        title 0MyService
        net stop 0MyService
        pause
        exit