using Redis.Library;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading;

namespace Redis.ConsoleApp.String
{
    /// <summary>
    /// Author: Gordon
    /// Created: 2019-10-17
    /// Description: Redis String Console Example
    /// </summary>
    class Program
    {
        static RedisConnectionFactory redisConnectionFactory = new RedisConnectionFactory(Config.RedisConnection);
        static IDatabase redis_db = redisConnectionFactory.GetConnection.GetDatabase(10);
        static RedisKey redis_key = "test";
        static RedisValue redis_value = DateTime.UtcNow.ToShortTimeString();
        static TimeSpan redis_timeSpan = new TimeSpan(hours: 0, minutes: 0, seconds: 30);
        static void Main(string[] args)
        {
            Console.WriteLine("Add Rediskey Data...");
            Redis();
            Console.WriteLine("Rest 1000 millisecond...");
            Thread.Sleep(1000);
            Console.WriteLine("Update Rediskey TimeSpan ...");
            Redis();
            Console.WriteLine("Rest 1000 millisecond...");
            Thread.Sleep(1000);

            Console.WriteLine("Get Rediskey TimeSpan ...");
            var redis_endPoint = redisConnectionFactory.GetConnection.GetEndPoints().First();
            var redis_server = redisConnectionFactory.GetConnection.GetServer(redis_endPoint);
            var redis_time = redis_db.KeyTimeToLive(redis_key);
            var redis_expire = redis_time == null ? (DateTime?)null : redis_server.Time().ToUniversalTime().Add(redis_time.Value); //返回UTC時間。
            Console.WriteLine(redis_key.ToString() + "->TimeSpan:" + redis_time + ";Expire date:" + redis_expire);


            Console.WriteLine("Delete Rediskey  ...");
            redis_db.KeyDelete(redis_key);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        public static void Redis()
        {

            if (redis_db.KeyExists(redis_key))
            {
                Console.WriteLine(redis_key.ToString() + " is exists");
            }
            else
            {
                Console.WriteLine(redis_key.ToString() + " is not exists");
            }


            if (redis_db.KeyExists(redis_key))
            {
                redis_value = redis_db.StringGet(redis_key);
                redis_db.StringSet(redis_key, redis_value, redis_timeSpan);

                Console.WriteLine(redis_key.ToString() + " is Update TimeSpan");
            }
            else
            {
                redis_db.StringSet(redis_key, redis_value, redis_timeSpan);

                Console.WriteLine(redis_key.ToString() + " is Add Data");
            }
        }
    }
}
