��ǰ���Ļ����������޸ĺ��Ż�,ǰ���Ĵ��������� : https://github.com/Coldairarrow/DotNettyRPC
    
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
    1.�������Ҫ���÷�����Լ,����ʵ��,ʵ��.����ʾ���е� ITest,Test,Person ����Ӧ�����;
    2.�ͻ�����Ҫ���÷�����Լ,ʵ��;
    3.�ͻ���������������,������,�����.
    4.Ĭ�����л���ʽΪ Json,����������һ��: MessagePack


��������:
    1.Task.Run() 10000 ��,ƽ��ÿ�κ�ʱ 0.12 ms;
    2.���߳����� 10000 ��,ƽ��ÿ�κ�ʱ 0.3 ms.
