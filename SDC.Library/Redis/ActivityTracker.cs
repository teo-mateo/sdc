using ServiceStack.Model;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDC.Library.Redis
{
    public class ActivityTracker
    {
        public static void TrackActive(string userName)
        {
            ServiceStack.Redis.RedisClient cli = new ServiceStack.Redis.RedisClient();
            var masterSet = cli.Sets["activity-master"];
            var currentSetKey = "activity-" + DateTime.Now.ToString("dd-hh-mm");
            masterSet.Add(currentSetKey);
            var set = cli.Sets[currentSetKey];
            set.Add(userName);
        }

        public static string[] GetActiveUsers()
        {
            ServiceStack.Redis.RedisClient cli = new ServiceStack.Redis.RedisClient();
            var masterList = cli.Sets["activity-master"];
            var masterKeys = masterList.GetAll();

            
            var lastSetKeys = GetActivitySets();
            string[] lastSetIds = lastSetKeys
                .Where(k => masterKeys.Contains(k))
                .Select(k => cli.Sets[k].Id).ToArray();

            return cli.GetUnionFromSets(lastSetIds).ToArray();
        }

        private static string[] GetActivitySets()
        {
            return (from i in Enumerable.Range(0, 5)
                    select "activity-" + DateTime.Now.Subtract(TimeSpan.FromMinutes(i))
                    .ToString("dd-hh-mm"))
                    .ToArray();
        }
    }
}
