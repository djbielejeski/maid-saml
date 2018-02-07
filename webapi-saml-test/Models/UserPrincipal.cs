using System.Security.Principal;
using webapi_saml_test_provider.Models;
using webapi_saml_test_shared.Models;

namespace webapi_saml_test_provider.Models
{
    public interface IUserPrincipal : IPrincipal
    {
        string UserID { get; }
        bool IsAuthenticated { get; }
    }

    public class UserPrincipal : IUserPrincipal
    {
        public string GroupID { get; set; }
        public UserType UserType { get; set; }
        public IIdentity Identity { get; private set; }

        public UserPrincipal(UserData userData)
        {
            this.Identity = new GenericIdentity(userData.UserId);
            this.UserType = userData.UserType;
        }

        public bool IsInRole(string role)
        {
            return Identity != null &&
                    Identity.IsAuthenticated &&
                    !string.IsNullOrWhiteSpace(role) &&
                    this.UserType.ToString() == role;
        }

        public string UserID
        {
            get
            {
                if (Identity != null &&
                    Identity.IsAuthenticated)
                {
                    return Identity.Name;
                }

                return string.Empty;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                if (this.Identity != null)
                {
                    return this.Identity.IsAuthenticated;
                }

                return false;
            }
        }
    }
}