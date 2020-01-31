��ǰ���Ļ����������޸ĺ��Ż�,ǰ���Ĵ��������� : https://github.com/Coldairarrow/DotNettyRPC

��    ��:   .NET Standard 2.0
ͨ    ��:   DotNetty
��̬����:   ImpromptuInterface
�� �� ��:   Newtonsort.Json,MessagePack
�� �� ��:   Microsoft.Extensions.ObjectPool
����ע��:   Microsoft.Extensions.DependencyInjection
��־��¼:   Wjire.Log (����)    


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
     Server server = new Server(7878);
     
     //.net core �Դ���DI����
     ServiceCollection services = new ServiceCollection();
     services.AddSingleton<ITest, Test>();

     server.RegisterServices(services);
     server.Start().Wait();

�ͻ���
         
     ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 7878);//�ڲ���������
     Person person = client.GetPerson(1);//�ڲ�ʵ��Ϊ������+�����.

��������:
    1.Task.Run() 10000 ��,ƽ��ÿ�κ�ʱ 0.12 ms;
    2.���߳����� 10000 ��,ƽ��ÿ�κ�ʱ 0.28 ms.
