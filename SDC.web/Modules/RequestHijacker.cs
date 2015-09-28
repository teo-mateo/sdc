using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDC.web.Modules
{
    public class RequestHijacker: IHttpModule
    {
        public void Dispose()
        {
            
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += Context_BeginRequest;
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            
        }
    }
}