using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace webapi_saml_test_client.Models
{
    public class GeneratedJWTModel
    {
        public string token { get; set; }
        public ClaimsPrincipal principle { get; set; }
    }
}