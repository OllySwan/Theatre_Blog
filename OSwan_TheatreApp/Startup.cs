using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OSwan_TheatreApp.Startup))]
namespace OSwan_TheatreApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
