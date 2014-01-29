using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NHibernate.AspNet.Web.Startup))]
namespace NHibernate.AspNet.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            DataConfig.Configure(System.Web.HttpContext.Current.ApplicationInstance);
        }
    }
}
