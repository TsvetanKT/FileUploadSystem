using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FileUploadSystem.Web.Startup))]
namespace FileUploadSystem.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
