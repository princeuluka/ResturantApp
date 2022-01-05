using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ResturantApp.Startup))]
namespace ResturantApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
