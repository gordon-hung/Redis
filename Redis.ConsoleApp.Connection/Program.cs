using Redis.Library;
using StackExchange.Redis;
using System;
using System.Diagnostics;

namespace Redis.ConsoleApp.Connection
{
    /// <summary>
    /// Author: Gordon
    /// Created: 2019-10-17
    /// Description: Redis Connection Console Example
    /// </summary>
    class Program
    {
        private static string redis_connection = Config.RedisConnection;
        private static RedisConnectionFactory redisConnectionFactory = new RedisConnectionFactory(Config.RedisConnection);
        private static IDatabase redis_db = redisConnectionFactory.GetConnection.GetDatabase(Config.RedisDatabase);
        private static TimeSpan redis_timeSpan = new TimeSpan(hours: 0, minutes: 0, seconds: Config.RedisTimeSpan);
        private static string message_format = "{0} times cost {1} milliseconds";
        private static SnowflakeIdGenerator snowflakeIdGenerator = new SnowflakeIdGenerator(1, 1);
        private static bool isRun = true;
        static void Main(string[] args)
        {
            while (isRun)
            {
                Console.WriteLine("Press 【1】 is Run or 【exit or 0】 close... ");
                var input = Console.ReadLine();
                if (input != "1")
                {
                    isRun = false;
                }
                else
                {
                    Console.Clear();
                    ExecuteRedis();
                }
            }
        }

        /// <summary>
        /// Execute Redis
        /// </summary>
        private static void ExecuteRedis()
        {
            Stopwatch stopwatch = new Stopwatch();

            Console.WriteLine("Use (using) connection speed.");
            for (int i = 0; i < 10; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redis_connection))
                {
                    var db = redis.GetDatabase(Config.RedisDatabase);
                    string redisKey = snowflakeIdGenerator.nextId().ToString();
                    string redisValue = Guid.NewGuid().ToString();
                    db.StringSet(redisKey, redisValue, redis_timeSpan);
                    Console.WriteLine("Show " + redisKey + " " + redis_db.StringGet(redisKey));
                }
                stopwatch.Stop();
                Console.WriteLine(string.Format(message_format, i + 1, stopwatch.ElapsedMilliseconds));
            }

            Console.WriteLine("Not use (using) connection speed.");
            for (int i = 0; i < 10; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();
                string redisKey = snowflakeIdGenerator.nextId().ToString();
                string redisValue = Guid.NewGuid().ToString();
                redis_db.StringSet(redisKey, redisValue, redis_timeSpan);
                Console.WriteLine("Show " + redisKey + " " + redis_db.StringGet(redisKey));
                stopwatch.Stop();
                Console.WriteLine(string.Format(message_format, i + 1, stopwatch.ElapsedMilliseconds));
            }
        }
    }
}
