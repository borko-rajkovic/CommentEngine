using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;
using CommentEngine;
[assembly: OwinStartup(typeof(CommentEngine.Startup))]

namespace CommentEngine
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
