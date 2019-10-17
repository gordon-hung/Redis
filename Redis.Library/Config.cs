using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Redis.Library
{
    /// <summary>
    /// Author: Gordon
    /// Created: 2019-10-17
    /// Description: Config Setting
    /// </summary>
    public class Config
    {
        public static IConfiguration Configuration { get; }

        static Config()
        {
            if (Configuration == null)
            {
                Configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            }
        }

        /// <summary>
        /// Redis連線。
        /// </summary>
        public static string RedisConnection
        {
            get
            {
                return Configuration.GetSection("RedisConnection").Get<string>();
            }
        }
    }
}
