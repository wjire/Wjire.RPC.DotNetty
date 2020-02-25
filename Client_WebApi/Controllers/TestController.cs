using System.Threading;
using System.Threading.Tasks;
using IServices;
using Microsoft.AspNetCore.Mvc;
using Wjire.RPC.Client;

namespace Client_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private static int num;
        public string Get()
        {
            int count = 1000;
            Task[] tasks = new Task[count];
            ClientConfig config = new ClientConfig();
            ITest client = ClientFactory.GetClient<ITest>(config);
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    Person person = client.GetPerson(Interlocked.Increment(ref num));
                });
            }
            Task.WaitAll(tasks);
            return num.ToString();
        }
    }
}