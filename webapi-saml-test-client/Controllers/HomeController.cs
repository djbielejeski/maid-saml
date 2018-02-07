using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using webapi_saml_test_client.Models;
using webapi_saml_test_shared.Utilities;

namespace webapi_saml_test_client.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string userId = "123456")
        {
            GeneratedSamlModel model = new GeneratedSamlModel();

            string CertPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/maid.pfx");
            X509Certificate2 SigningCert = new X509Certificate2(CertPath, "MaidRocks1");

            Dictionary<string, string> SAMLAttributes = new Dictionary<string, string>();
            SAMLAttributes.Add("UserId", userId);

            model.EncryptedSaml = SAML20Assertion.CreateSAML20Response("maid.com", 60, "Audience", string.Empty, "Recipient", SAMLAttributes, SigningCert);
            model.DecryptedSaml = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(model.EncryptedSaml));

            ViewBag.userId = userId;
            return View(model);
        }
    }
}