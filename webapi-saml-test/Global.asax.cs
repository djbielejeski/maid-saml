using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using webapi_saml_test.Utilities;
using webapi_saml_test_provider.Filters;
using webapi_saml_test_shared.Utilities;

namespace webapi_saml_test_provider
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_OnPostAuthenticateRequest(object sender, EventArgs e)
        {
            if (!Request.FilePath.StartsWith("/api/", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            ClaimsPrincipal userPrinciple = null;
            try
            {
                // See if they have the JsonWebToken header
                string token = HttpContext.Current.Request.Headers.GetValues(ConfigurationManager.AppSettings["header-token-name"]).FirstOrDefault();

                if (!string.IsNullOrEmpty(token))
                {
                    // parse the token and setup the principle context
                    userPrinciple = JsonWebTokenUtility.DecodeToken(token, CertificateHelper.GetCertificate());
                }
            }
            catch
            {
                // Noop
            }

            if (userPrinciple == null)
            {
                return;
            }
            else
            {
                Thread.CurrentPrincipal = userPrinciple;
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = Thread.CurrentPrincipal;
                }
            }
        }

        
    }
}
