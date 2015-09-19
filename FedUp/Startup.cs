using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FedUp.Startup))]
namespace FedUp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
