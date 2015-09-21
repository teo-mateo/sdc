using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using System.Collections.Concurrent;

namespace SDC.Library.Controllers
{
    /// <summary>
    /// logs controller actions to redis
    /// </summary>
    public class ControllerActionLogger
    {
        public string SetName
        {
            get { return _setName; }
        }

        public ConcurrentDictionary<string, DateTime> _dict;
        public string _setName;
        public ControllerActionLogger(string setName = "controller:log")
        {
            _setName = setName;
            _dict = new ConcurrentDictionary<string, DateTime>();
        }

        public void StartAction(string identifier)
        {
            try
            {
                bool added = false;
                do
                {
                    added = _dict.TryAdd(identifier, DateTime.Now);
                } while (!added);
            }
            catch (Exception)
            {
                //NOOP 
            }
        }

        public void EndAction(string identifier, string actionName, string controllerName, string info)
        {
            try
            {
                DateTime now = DateTime.Now;
                DateTime dt = DateTime.Now;
                if (_dict.Keys.Contains(identifier))
                {
                    bool retrieved = false;
                    do
                    {
                        retrieved = _dict.TryRemove(identifier, out dt);
                    } while (!retrieved);
                }

                TimeSpan duration = now.Subtract(dt);

                //get the redis client and the controller log set
                IRedisClient cli = SDCApp.Kernel.Get<IRedisClient>();
                var set = cli.Sets[_setName];

                //get the string representation of log item.
                string value = JsonConvert.SerializeObject(new ControllerActionLogItem()
                {
                    Controller = controllerName,
                    Action = actionName,
                    Duration = duration.Milliseconds,
                    Start = dt,
                    End = now
                });

                //push to redis
                set.Add(value);
            }
            catch (Exception)
            {
                //NOOP
            }

        }
    }

    public class ControllerActionLogItem
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Duration { get; set; }
        public string Info { get; set; }
    }
}
