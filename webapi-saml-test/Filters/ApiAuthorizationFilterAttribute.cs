using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using webapi_saml_test_provider.Models;
using webapi_saml_test_shared.Models;

namespace webapi_saml_test_provider.Filters
{
    public class ApiAuthorization : AuthorizeAttribute
    {
        private List<UserType> _rights;
        public ApiAuthorization(params UserType[] rights)
        {
            if (rights == null)
            {
                throw new ArgumentNullException("rights");
            }

            _rights = rights.ToList();
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return _rights.Any(x => Thread.CurrentPrincipal.IsInRole(x.ToString()));
        }
    }
}