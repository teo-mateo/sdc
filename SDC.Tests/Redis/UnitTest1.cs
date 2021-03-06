﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using SDC.Library.Redis;
using ServiceStack.Redis;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SDC.Tests.Redis
{
    [TestClass]
    public class RedisTests
    {
        [TestMethod]
        public void Simple()
        {
            ServiceStack.Redis.RedisClient cli = new ServiceStack.Redis.RedisClient();
            var c1 = cli.As<UserLight>();
            var set1 = c1.Sets["set1"];

            //set1.Add(new UserLight() { Id = DateTime.Now.Ticks, UserName = "user1", LastSeen = DateTime.Now });

            var s = "activity-"+DateTime.Now.ToString("dd-hh-mm-ss");

            var users = ActivityTracker.GetActiveUsers();
        }

        [TestMethod]
        public void Union_Sets_Test()
        {
            RedisClient cli = new RedisClient();
            var set1 = cli.Sets["uniontests-1"];
            var set2 = cli.Sets["uniontests-2"];
            var set3 = cli.Sets["uniontests-3"];

            set1.Add("1");
            set1.Add("1");
            set1.Add("2");
            set1.Add("3");
            set1.Add("2");

            set2.Add("9");
            set2.Add("10");
            set2.Add("9");
            set2.Add("10");
            set2.Add("1");

            set3.Add("5");

            RedisClient cli2 = new RedisClient();
            var set1a = cli2.Sets["uniontests-1"];
            var set2a = cli2.Sets["uniontests-2"];
            var set3a = cli2.Sets["uniontests-3"];

            var unionResult = set1a.Union(new IRedisSet[]{ set2a, set3a});

            Assert.IsTrue(unionResult.Count == 6);
        }

        [TestMethod]
        public void CleanActivityData()
        {
            RedisClient cli = new RedisClient();
            var master = cli.Sets["activity-master"];
            var sets = master.GetAll();
            foreach(var set in sets)
            {
                cli.Remove(set);
                master.Remove(set);
            }
        }

        [TestMethod]
        public void Performance_SADD()
        {
            string[] s = Enumerable.Range(1, 1000*1000).Select(i => Guid.NewGuid().ToString()).ToArray();
            Debug.WriteLine("Source array size: " + Utility.U.ObjectSize(s));

            PooledRedisClientManager prcm = new PooledRedisClientManager();
            
            IRedisClient cli = prcm.GetClient();
            cli.ConnectTimeout = 1000;

            cli.Delete("tests-sadd");
            var set = cli.Sets["tests-sadd"];

            int adds = 0;

            using (var c = new Utility.U.Chrono("SADD " + s.Length))
            {
                foreach (var i in s)
                {
                    set.Add(i);
                    adds++;
                }
            }
            
        }
    }

    public class UserLight
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
