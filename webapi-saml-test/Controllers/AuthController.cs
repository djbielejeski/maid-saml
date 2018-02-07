using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Web.Http;
using System.Xml;
using System.Xml.Serialization;
using webapi_saml_test_provider.Models;
using webapi_saml_test_shared.Models;
using webapi_saml_test_shared.Utilities;

namespace webapi_saml_test_provider.Controllers
{
    [RoutePrefix("api/v1/auth")]
    public class AuthController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="EncryptedSamlPayload"></param>
        /// <param name="RelayState">This is where the user wants to go if this request is valid.</param>
        /// <returns></returns>
        [HttpPost, Route("")]
        public string Post([FromBody]string EncryptedSamlPayload) //, string RelayState
        {
            if (string.IsNullOrEmpty(EncryptedSamlPayload))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            string decriptedSamlString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(EncryptedSamlPayload));
            XmlDocument samlAsXml = new XmlDocument();
            samlAsXml.LoadXml(decriptedSamlString);

            // Make sure the request was created with the correct certificate.
            if (!ValidateX509CertificateSignature(samlAsXml))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            // Parse the data from the SAML request
            AssertionType assertion = GetAssertionFromXMLDoc(samlAsXml);

            //if (assertion.Issuer.Value == ConfigurationManager.AppSettings["CertIssuer"])
            if (assertion.Issuer.Value == ConfigurationManager.AppSettings["origin-site"])
            {
                SAMLData SSOData = new SAMLData(assertion);


                // At this point any specific work that needs to be done to establish user context with
                // the SSOData should be executed before redirecting the user browser to the target
                List<string> validUserIds = new List<string>{ "123456", "789012", "555555" };

                if (validUserIds.Contains(SSOData.UserData.UserId))
                {
                    // Setup the SAML token in the headers
                    return JsonWebTokenUtility.CreateToken(SSOData.UserData);
                }
            }

            return string.Empty;
        }

        private AssertionType GetAssertionFromXMLDoc(XmlDocument SAMLXML)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(SAMLXML.NameTable);
            ns.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            XmlElement xeAssertion = SAMLXML.DocumentElement.SelectSingleNode("saml:Assertion", ns) as XmlElement;

            XmlSerializer serializer = new XmlSerializer(typeof(AssertionType));

            AssertionType assertion = (AssertionType)serializer.Deserialize(new XmlNodeReader(xeAssertion));

            return assertion;
        }

        private bool ValidateX509CertificateSignature(XmlDocument SAMLResponse)
        {
            XmlNodeList XMLSignatures = SAMLResponse.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#");

            // Checking If the Response or the Assertion has been signed once and only once.
            if (XMLSignatures.Count != 1) return false;
            SignedXml SignedSAML = new SignedXml(SAMLResponse);
            SignedSAML.LoadXml((XmlElement)XMLSignatures[0]);

            String CertPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/maid.cer");
            X509Certificate2 SigningCert = new X509Certificate2(CertPath);

            return SignedSAML.CheckSignature(SigningCert, true);
        }
    }
}
