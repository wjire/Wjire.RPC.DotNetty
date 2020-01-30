﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Wjire.RPC.DotNetty.Client;

namespace Client_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private static int num;
        public string Get()
        {
            var count = 1000;
            Task[] tasks = new Task[count];
            ClientConfig config = new ClientConfig("127.0.0.1", 7878)
            {
                AllIdleTimeSeconds = 10
            };
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