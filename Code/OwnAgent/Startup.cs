using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OwnAgent.Startup))]
namespace OwnAgent
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}