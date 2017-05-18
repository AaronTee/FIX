using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FIX.Web.Startup))]
namespace FIX.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
