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
        static string redis_connection = Config.RedisConnection;
        static RedisConnectionFactory redisConnectionFactory = new RedisConnectionFactory(Config.RedisConnection);
        static string message_format = "{0} times cost {1} milliseconds";

        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();

            Console.WriteLine("Use (using) connection speed.");
            for (int i = 0; i < 10; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redis_connection))
                {
                    var db = redis.GetDatabase(db: 0);
                    db.StringSet(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DateTime.Now.ToShortTimeString(), new TimeSpan(0, 0, 3));
                }
                stopwatch.Stop();
                Console.WriteLine(string.Format(message_format, i + 1, stopwatch.ElapsedMilliseconds));
            }

            Console.WriteLine("Unused (using) connection speed.");
            for (int i = 0; i < 10; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();
                var db = redisConnectionFactory.GetConnection.GetDatabase(db: 0);
                db.StringSet(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DateTime.Now.ToShortTimeString(), new TimeSpan(0, 0, 3));
                stopwatch.Stop();
                Console.WriteLine(string.Format(message_format, i + 1, stopwatch.ElapsedMilliseconds));
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
