using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Section106.Startup))]
namespace Section106
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
