using log4net.Appender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using System.Diagnostics;

namespace RedisAppender
{
    public class RedisAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            Debug.WriteLine("append logging event to redis: " + loggingEvent.MessageObject);

            
        }
    }
}
