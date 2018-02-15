using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace webapi_saml_test.Utilities
{
    public static class CertificateHelper
    {
        public static X509Certificate2 GetCertificate()
        {
            string certLocation = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/maid.pfx");
            return new X509Certificate2(certLocation, ConfigurationManager.AppSettings["certPassword"]);
        }
    }
}