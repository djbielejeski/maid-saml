using System.Web.Http;
using webapi_saml_test.Utilities;
using webapi_saml_test_shared.Utilities;

namespace webapi_saml_test_provider.Controllers
{
    [RoutePrefix("api/v1/auth")]
    public class AuthController : ApiController
    {
        [HttpGet, Route("")]
        public string Get([FromUri]string emailAddress = "david@gmail.com")
        {
            return JsonWebTokenUtility.CreateToken(emailAddress, CertificateHelper.GetCertificate());
        }
    }
}
