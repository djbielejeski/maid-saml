using Microsoft.IdentityModel.Tokens;
using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace webapi_saml_test_shared.Utilities
{
    public static class JsonWebTokenUtility
    {
        public static string CreateToken(string emailAddress, X509Certificate2 cert)
        {
            Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Email, emailAddress) }),
                Audience = "maid",
                Issuer = "maid",
                Expires = DateTime.UtcNow.AddHours(Convert.ToInt32(ConfigurationManager.AppSettings["tokenLengthInHours"])),
                SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(new X509SecurityKey(cert), Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            Microsoft.IdentityModel.Tokens.SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        public static ClaimsPrincipal DecodeToken(string token, X509Certificate2 cert)
        {
            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new X509SecurityKey(cert),
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
    }
}