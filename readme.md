��ǰ���Ļ����������޸ĺ��Ż�,ǰ���Ĵ��������� : https://github.com/Coldairarrow/DotNettyRPC

��    ��:   .NET Standard 2.0
ͨ    ��:   DotNetty.Handlers,DotNetty.Transport
��̬����:   ImpromptuInterface
�� �� ��:   Newtonsort.Json,MessagePack
�� �� ��:   Microsoft.Extensions.ObjectPool
����ע��:   Microsoft.Extensions.DependencyInjection
��־��¼:   Wjire.Log (����)    


ʾ��:
�����
     Server server = new Server(7878);
     
     //.net core �Դ���DI����
     ServiceCollection services = new ServiceCollection();
     services.AddSingleton<ITest, Test>();

     server.RegisterServices(services);
     server.Start().Wait();

�ͻ���
         
     ITest client = ClientFactory.GetClient<ITest>("127.0.0.1", 7878);
     Person person = client.GetPerson(1);

PS:
    1.�ͻ���������������,������,�����.
    2.Ĭ�����л���ʽΪ Json,����������һ��: MessagePack


��������:
    1.Task.Run() 10000 ��,ƽ��ÿ�κ�ʱ 0.12 ms;
    2.���߳����� 10000 ��,ƽ��ÿ�κ�ʱ 0.3 ms.
