using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Wjire.RPC.DotNetty.Client;

namespace Wjire.RPC.DotNetty.Helper
{

    /// <summary>
    /// 配置文件操作类
    /// </summary>
    internal static class ConfigurationHelper
    {
        private const string Development = "appsettings.Development.json";
        private const string Release = "appsettings.json";
        private const string ClientConfigKeyInAppSettings = "ClientConfig";
        private static readonly IConfigurationRoot Config;

        static ConfigurationHelper()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(baseDirectory);
            string path = Path.Combine(baseDirectory, Development);
            if (File.Exists(path))
            {
                Config = builder.AddJsonFile(Development, false, true).Build();
            }
            else
            {
                Config = builder.AddJsonFile(Release, false, true).Build();
            }
        }


        /// <summary>
        /// 读取配置文件,获取 ClientConfig
        /// </summary>
        /// <returns></returns>
        internal static ClientConfig GetClientConfig()
        {
            ClientConfig clientConfig = Config.GetSection(ClientConfigKeyInAppSettings).Get<ClientConfig>();
            if (clientConfig == null)
            {
                throw new ArgumentException($"配置文件中未找到 {ClientConfigKeyInAppSettings} 节点");
            }
            return clientConfig;
        }
    }
}
