using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Fondef.Startup))]
namespace Fondef
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
