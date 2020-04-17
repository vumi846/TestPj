using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(VNPTPM.Web.Startup))]

namespace VNPTPM.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //local use below else comment
            //app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        }
    }
}
