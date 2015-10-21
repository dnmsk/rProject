using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Project_B.Startup))]
namespace Project_B
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
