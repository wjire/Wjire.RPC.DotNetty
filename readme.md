��ǰ���Ļ����������޸ĺ��Ż�,ǰ���Ĵ��������� : https://github.com/Coldairarrow/DotNettyRPC

��    ��:   .NET Standard 2.0

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


�����ÿ���̨,Ҳ��������windows����.

Windows ����������

    ע��:
        
        @echo off
        title ServiceName
        sc delete ServiceName
        sc create ServiceName binpath= "%~dp0%..\******.exe" displayname= "ServiceName" depend= Tcpip start= auto
        sc description ServiceName ��������
        pause
        exit


    ����:

        @echo off
        title ServiceName
        net start ServiceName
        pause
        exit

    ֹͣ:

        @echo off
        title ServiceName
        net stop ServiceName
        pause
        exit