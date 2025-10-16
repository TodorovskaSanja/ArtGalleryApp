using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ArtGalleryApp.Startup))]
namespace ArtGalleryApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
