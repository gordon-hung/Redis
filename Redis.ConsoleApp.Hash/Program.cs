﻿using Redis.Library;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.ConsoleApp.Hash
{
    /// <summary>
    /// Author: Gordon
    /// Created: 2019-10-17
    /// Description: Redis Hash Console Example
    /// </summary>
    class Program
    {
        private static int Count = 10;
        private static RedisConnectionFactory redisConnectionFactory = new RedisConnectionFactory(Config.RedisConnection);
        private static IDatabase redis_db = redisConnectionFactory.GetConnection.GetDatabase(Config.RedisDatabase);
        private static TimeSpan redis_timeSpan = new TimeSpan(hours: 0, minutes: 0, seconds: Config.RedisTimeSpan);
        private static RedisKey redis_key = "testHash";
        private static List<HashEntry> hashEntry = new List<HashEntry>();
        private static SnowflakeIdGenerator snowflakeIdGenerator = new SnowflakeIdGenerator(1, 1);
        private static bool isRun = true;

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
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
            for (int i = 0; i < Count; i++)
            {
                hashEntry.Add(new HashEntry(snowflakeIdGenerator.nextId(), Guid.NewGuid().ToString()));
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
            redis_db.HashSet(redis_key, hashEntry.ToArray());

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
            redis_db.HashGetAll(redis_key).ToList().ForEach(item => Console.WriteLine("Show " + redis_key.ToString() + " " + item));

            Console.WriteLine("Update Rediskey Value ...");
            hashEntry.ForEach(item =>
           {
               string value = item.Value;
               value += ":" + UnixTimestamp.ConvertToTimestamp(DateTime.Now);
               redis_db.HashSet(redis_key, hashField: item.Name, value);
           });

            Console.WriteLine("GetMany Rediskey...");
            redis_db.HashGetAll(redis_key).ToList().ForEach(item => Console.WriteLine("Show " + redis_key.ToString() + " " + item));

            Console.WriteLine("Get Rediskey TimeSpan ...");
            var redis_endPoint = redisConnectionFactory.GetConnection.GetEndPoints().First();
            var redis_server = redisConnectionFactory.GetConnection.GetServer(redis_endPoint);
            var redis_time = redis_db.KeyTimeToLive(redis_key);
            var redis_expire = redis_time == null ? (DateTime?)null : redis_server.Time().ToUniversalTime().Add(redis_time.Value); //返回UTC時間。
            Console.WriteLine(redis_key.ToString() + "->TimeSpan:" + redis_time + ";Expire date:" + redis_expire);

            Console.WriteLine("Delte Rediskey Content...");
            redis_db.HashGetAll(redis_key).ToList().ForEach(item => redis_db.HashDelete(redis_key, hashField: item.Value));

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
        }
    }
}
