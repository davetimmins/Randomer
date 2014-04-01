using Funq;
using ServiceStack;
using ServiceStack.Caching;
using ServiceStack.MiniProfiler;
using ServiceStack.Razor;
using ServiceStack.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace TestWeb
{
    public class AppHost : AppHostBase
    {
        public AppHost() : base("ArcGIS Online Hosted Data Services", typeof(AppHost).Assembly) { }

        public override void Configure(Container container)
        {
            container.Register<ICacheClient>(new MemoryCacheClient());

            Plugins.Add(new RazorFormat());
            // Plugins.Add(new RequestLogsFeature());

            Plugins.Add(new ValidationFeature());
            container.RegisterValidators(typeof(TestWeb.ServiceInterface.RandomDataService).Assembly);
        }
    }

    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            new AppHost().Init();
        }

        protected void Application_BeginRequest(object src, EventArgs e)
        {
            if (Request.IsLocal)
                Profiler.Start();
        }

        protected void Application_EndRequest(object src, EventArgs e)
        {
            Profiler.Stop();
        }
    }
}