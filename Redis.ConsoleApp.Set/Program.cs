using Redis.Library;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Redis.ConsoleApp.Set
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
        static List<Hash<string>> hashList = new List<Hash<string>>();
        static void Main(string[] args)
        {
            for (int i = 0; i < 1000; i++)
            {
                hashList.Add(new Hash<string>() { key = Guid.NewGuid().ToString(), value = DateTime.Now.ToShortTimeString() });
            }

            if (redis_db.KeyExists(redis_key))
            {
                Console.WriteLine(redis_key.ToString() + " is exists");
                Console.WriteLine("Delete Rediskey  ...");
                redis_db.KeyDelete(redis_key);
            }
            else
            {
                Console.WriteLine(redis_key.ToString() + " is not exists");
            }

            Console.WriteLine("Add Rediskey Data...");
            foreach (var hash in hashList)
            {
                redis_db.SetAdd(redis_key, hash.key);
            }
            Console.WriteLine("Rest 1000 millisecond...");
            Thread.Sleep(1000);
            Console.WriteLine("Remove Rediskey TimeSpan ...");
            foreach (var hash in hashList)
            {
                redis_db.SetRemove(redis_key, hash.key);
            }
            Console.WriteLine("Rest 1000 millisecond...");
            Thread.Sleep(1000);

            Console.WriteLine("Get Rediskey TimeSpan ...");
            var redis_endPoint = redisConnectionFactory.GetConnection.GetEndPoints().First();
            var redis_server = redisConnectionFactory.GetConnection.GetServer(redis_endPoint);
            var redis_time = redis_db.KeyTimeToLive(redis_key);
            var redis_expire = redis_time == null ? (DateTime?)null : redis_server.Time().ToUniversalTime().Add(redis_time.Value); //返回UTC時間。
            Console.WriteLine(redis_key.ToString() + "->TimeSpan:" + redis_time + ";Expire date:" + redis_expire);


            if (redis_db.KeyExists(redis_key))
            {
                Console.WriteLine(redis_key.ToString() + " is exists");
                Console.WriteLine("Delete Rediskey  ...");
                redis_db.KeyDelete(redis_key);
            }
            else
            {
                Console.WriteLine(redis_key.ToString() + " is not exists");
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        public class Hash<Tsoure>
        {
            /// <summary>
            /// 流水號
            /// </summary>
            public string key { get; set; }
            /// <summary>
            /// 值
            /// </summary>
            public Tsoure value { get; set; }
        }
    }
}
