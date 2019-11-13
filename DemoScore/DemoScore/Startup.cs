using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DemoScore.Startup))]
namespace DemoScore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
