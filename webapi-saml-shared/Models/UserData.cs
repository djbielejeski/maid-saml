using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webapi_saml_test_shared.Models
{
    public class UserData
    {
        // Any changes here need to be made in 
        public string UserId { get; set; }
        public UserType UserType { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}