��ǰ���Ļ����������޸ĺ��Ż�,ǰ���Ĵ��������� : https://github.com/Coldairarrow/DotNettyRPC

��    ��:   .NET Standard 2.0

��:

  <ItemGroup>
    <!--ͨ�ſ��-->
    <PackageReference Include="DotNetty.Codecs" Version="0.6.0" />
    <PackageReference Include="DotNetty.Transport" Version="0.6.0" />
    <!--��̬����-->
    <PackageReference Include="ImpromptuInterface" Version="7.0.1" />
    <!--���л�-->
    <PackageReference Include="Newtonsoft.Json" Version="12.0.3" />
    <PackageReference Include="MessagePack" Version="2.1.90" />
    <!--������windows����-->
    <PackageReference Include="Microsoft.Extensions.Hosting.WindowsServices" Version="3.1.2" />
    <!--���ӳ�-->
    <PackageReference Include="Microsoft.Extensions.ObjectPool" Version="3.1.2" />
    <!--��־��¼-->
    <PackageReference Include="Wjire.Log" Version="1.0.3" />

    <PackageReference Include="Microsoft.Extensions.Hosting" Version="3.1.2" />
    <PackageReference Include="Microsoft.Extensions.Hosting.Abstractions" Version="3.1.2" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="3.1.2" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="3.1.2" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="3.1.2" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="3.1.2" />
    <PackageReference Include="Microsoft.Extensions.Hosting.Abstractions" Version="3.1.2" />
  </ItemGroup>


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