using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(XRayHub.Startup))]
namespace XRayHub
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
