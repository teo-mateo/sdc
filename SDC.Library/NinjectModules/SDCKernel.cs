﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using log4net;
using ServiceStack.Redis;

namespace SDC.Library.NinjectModules
{
    public class SDCModule : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            //bind stuff to stuff here.
            this.Bind<IRedisClient>().ToConstant(new RedisClient());
        }
    }
}
