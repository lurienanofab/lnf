using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using LNF;

namespace LNF.Scripting
{
    public class ScriptingModule : IHttpModule
    {
        private HttpApplication app;

        public void Init(HttpApplication context)
        {
            app = context;
            app.BeginRequest += new EventHandler(app_BeginRequest);
            app.EndRequest += new EventHandler(app_EndRequest);
        }

        public void Dispose()
        {
            
        }

        private void app_BeginRequest(object sender, EventArgs e)
        {
            Engine engine = ScriptingContext.Engine;
        }

        private void app_EndRequest(object sender, EventArgs e)
        {
            ScriptingContext.Dispose();
        }
    }
}
