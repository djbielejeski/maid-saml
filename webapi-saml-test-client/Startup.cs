using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(webapi_saml_test_client.Startup))]
namespace webapi_saml_test_client
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
