using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redis.Library
{
    /// <summary>
    /// Author: Gordon
    /// Created: 2019-10-17
    /// Description: Redis Connection Factory
    /// </summary>
    public class RedisConnectionFactory
    {
        private readonly Lazy<ConnectionMultiplexer> Connection;
        private string connectionString { get; set; } = "localhost:6379";

        public RedisConnectionFactory(string _connectionString)
        {
            connectionString = _connectionString;
            var options = ConfigurationOptions.Parse(connectionString);
            Connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }
        public ConnectionMultiplexer GetConnection => Connection.Value;

        public IDatabase RedisDB => GetConnection.GetDatabase();
    }
}
