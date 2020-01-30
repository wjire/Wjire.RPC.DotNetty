using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Wjire.RPC.DotNetty.Helper
{
    /// <summary>
    /// appsettings.json 配置文件操作类
    /// </summary>
    public static class ConfigurationHelper
    {

        /// <summary>
        /// Config
        /// </summary>
        public static readonly IConfigurationRoot Config;

        public const string ProAppsettings = "appsettings.json";
        public const string DevAppsettings = "appsettings.Development.json";

        static ConfigurationHelper()
        {
            if (Config != null)
            {
                return;
            }

            ConfigurationBuilder builder = new ConfigurationBuilder();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DevAppsettings);
            if (File.Exists(path) == false)
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ProAppsettings);
            }
            builder.AddJsonFile(path, false, true);
            Config = builder.Build();
        }


        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns>string or null</returns>
        public static string GetString(string key)
        {
            try
            {
                return Config[key];
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Object or null</returns>
        public static T GetObject<T>(string key) where T : class, new()
        {
            try
            {
                return Config.GetValue<T>(key);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}