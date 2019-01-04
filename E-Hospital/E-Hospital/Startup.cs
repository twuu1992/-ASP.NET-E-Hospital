using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(E_Hospital.Startup))]
namespace E_Hospital
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
