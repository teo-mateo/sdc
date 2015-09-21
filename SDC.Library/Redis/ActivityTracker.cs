using ServiceStack.Model;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace SDC.Library.Redis
{
    public class ActivityTracker
    {
        /// <summary>
        /// pushes the username to Redis
        /// </summary>
        public static void TrackActive(string userName)
        {
            try
            {
                IRedisClient cli = SDCApp.Kernel.Get<IRedisClient>();
                var masterSet = cli.Sets["activity-master"];
                var currentSetKey = "activity-" + DateTime.Now.ToString("dd-hh-mm");
                masterSet.Add(currentSetKey);
                var set = cli.Sets[currentSetKey];
                set.Add(userName);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// fetches the active users from Redis
        /// </summary>
        public static string[] GetActiveUsers()
        {
            try
            {
                IRedisClient cli = SDCApp.Kernel.Get<IRedisClient>();
                var masterList = cli.Sets["activity-master"];
                var masterKeys = masterList.GetAll();

                var lastSetKeys = GetActivitySets();
                string[] lastSetIds = lastSetKeys
                    .Where(k => masterKeys.Contains(k))
                    .Select(k => cli.Sets[k].Id).ToArray();

                return cli.GetUnionFromSets(lastSetIds).ToArray();
            }
            catch (Exception)
            {
                return new string[0];
            }
        }

        /// <summary>
        /// Returns the names of the last 5 activity sets
        /// </summary>
        private static string[] GetActivitySets()
        {
            return (from i in Enumerable.Range(0, 5)
                    select "activity-" + DateTime.Now.Subtract(TimeSpan.FromMinutes(i))
                    .ToString("dd-hh-mm"))
                    .ToArray();
        }
    }
}
