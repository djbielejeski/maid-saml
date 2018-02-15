using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using webapi_saml_test_client.Models;
using webapi_saml_test_shared.Utilities;

namespace webapi_saml_test_client.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index(string emailAddress = "david@gmail.com")
        {
            GeneratedJWTModel model = new GeneratedJWTModel();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["providerUrl"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string tokenFromProvider = "";

            HttpResponseMessage response = await client.GetAsync("?emailAddress=" + emailAddress);
            if (response.IsSuccessStatusCode)
            {
                tokenFromProvider = await response.Content.ReadAsStringAsync();
                tokenFromProvider = tokenFromProvider.Replace("\"", "");
            }

            model.token = tokenFromProvider;

            // Attempt to decrypt the token with our client cert
            model.principle = ReadTokenWithClientCertificate(tokenFromProvider);

            ViewBag.emailAddress = emailAddress;
            return View(model);
        }

        private ClaimsPrincipal ReadTokenWithClientCertificate(string token)
        {
            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new X509SecurityKey(GetCertificate()),
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                ValidIssuer = "maid",
                ValidAudience = "maid"
            };

            Microsoft.IdentityModel.Tokens.SecurityToken validatedToken;

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

            return claimsPrincipal;
        }

        private X509Certificate2 GetCertificate()
        {
            string certLocation = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/maid.cer");
            return new X509Certificate2(certLocation);
        }
    }
}