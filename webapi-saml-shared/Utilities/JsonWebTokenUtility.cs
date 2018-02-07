using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using webapi_saml_test_shared.Models;

namespace webapi_saml_test_shared.Utilities
{
    public static class JsonWebTokenUtility
    {
        //https://github.com/jwt-dotnet/jwt 

        private const string key = "UserData";

        public static string CreateToken(UserData userData)
        {
            IDateTimeProvider provider = new UtcDateTimeProvider();
            var expirationDate = provider.GetNow().AddSeconds((userData.ExpirationDate - userData.IssuedDate).TotalSeconds);

            var unixEpoch = JwtValidator.UnixEpoch; // 1970-01-01 00:00:00 UTC
            var secondsSinceEpoch = Math.Round((expirationDate - unixEpoch).TotalSeconds);

            var payload = new Dictionary<string, object>
            {
                { key, userData },
                { "exp", secondsSinceEpoch }
            };

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            return encoder.Encode(payload, ConfigurationManager.AppSettings["jwtSecret"]);
        }

        public static UserData DecodeToken(string token)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                IDictionary<string, object> payload = decoder.DecodeToObject(token, ConfigurationManager.AppSettings["jwtSecret"], verify: true);
                UserData userData = Newtonsoft.Json.JsonConvert.DeserializeObject<UserData>(payload[key].ToString());
                return userData;
            }
            catch (TokenExpiredException)
            {
                Console.WriteLine("Token has expired");
            }
            catch (SignatureVerificationException)
            {
                Console.WriteLine("Token has invalid signature");
            }
            catch
            {
                // No op
            }

            return null;
        }
    }
}