using Redis.Library;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Redis.ConsoleApp.Hash
{
    /// <summary>
    /// Author: Gordon
    /// Created: 2019-10-17
    /// Description: Redis Hash Console Example
    /// </summary>
    class Program
    {
        static RedisConnectionFactory redisConnectionFactory = new RedisConnectionFactory(Config.RedisConnection);
        static IDatabase redis_db = redisConnectionFactory.GetConnection.GetDatabase(10);
        static RedisKey redis_key = "test";
        static RedisValue redis_value = DateTime.UtcNow.ToShortTimeString();
        static TimeSpan redis_timeSpan = new TimeSpan(hours: 0, minutes: 0, seconds: 30);
        static List<Hash<string>> hashList = new List<Hash<string>>();
        static HashEntry[] hashEntry = new HashEntry[hashList.Count];
        static void Main(string[] args)
        {
            for (int i = 0; i < 1000; i++)
            {
                hashList.Add(new Hash<string>() { key = Guid.NewGuid().ToString(), value = DateTime.Now.ToShortTimeString() });
            }
            hashEntry = new HashEntry[hashList.Count];
            int hashListCount = 0;

            hashList.ForEach(item =>
            {
                hashEntry[hashListCount] = new HashEntry(item.key, item.value);

                hashListCount++;
            });

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
            RedisHashSetAdd();

            Console.WriteLine("Rediskey Set TimeSpan...");
            if (redis_db.KeyExists(redis_key))
            {
                redis_db.KeyExpire(redis_key, redis_timeSpan);
            }
            else
            {
                Console.WriteLine(redis_key.ToString() + " is not exists");
            }

            Console.WriteLine("GetMany Rediskey...");
            RedisHashGetMany();

            Console.WriteLine("Update Rediskey Value ...");
            RedisHashSetUpdate();

            Console.WriteLine("GetMany Rediskey...");
            RedisHashGetMany();

            Console.WriteLine("Delte Rediskey Key...");
            RedisHashSetDelete();

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

        public static void RedisHashSetAdd()
        {
            redis_db.HashSet(redis_key, hashEntry);
        }

        public static void RedisHashSetUpdate()
        {
            hashList.ForEach(item =>
            {
                item.value = "Update " + DateTime.Now.ToLongTimeString();
                redis_db.HashSet(redis_key, hashField: item.key, item.value);
            });
        }

        public static void RedisHashSetDelete()
        {
            int Max = hashList.Count() - 1;
            int step = 0;
            hashList.ForEach(item =>
            {
                if (step >= Max)
                {
                    return;
                }
                Console.WriteLine("Delete" + redis_key.ToString() + " " + item.key);
                redis_db.HashDelete(redis_key, hashField: item.key);
                step++;
            });
        }

        public static void RedisHashGetMany()
        {
            redis_db.HashGetAll(redis_key).ToList().ForEach(item => Console.WriteLine("Show " + redis_key.ToString() + " " + item));
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
