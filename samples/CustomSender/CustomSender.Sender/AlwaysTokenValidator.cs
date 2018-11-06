using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace CustomSender.Sender
{
    internal class AlwaysTokenValidator : ISecurityTokenValidator
    {
        public bool CanValidateToken => true;

        public int MaximumTokenSizeInBytes { get; set; }

        public bool CanReadToken(string securityToken)
        {
            return true;
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            // basically validates EVERY token and just uses the name in the token for the username!
// #warning The AlwaysTokenValidator isn't meant for any production workload
            validatedToken = new JwtSecurityToken(securityToken);

            var match = Regex.Match(securityToken, "[^\\.]*\\.([^\\.]*)\\.[^\\.]*");

            string token64 = match.Groups[1].Value;
            string token = System.Text.Encoding.Default.GetString(Convert.FromBase64String(token64));

            dynamic tokenObj = JsonConvert.DeserializeObject(token);
            string name = tokenObj.name;

            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] {
                        new Claim(ClaimTypes.NameIdentifier, name),
                        new Claim(ClaimTypes.Name, name),
                        new Claim(ClaimsIdentity.DefaultNameClaimType, name)
                    }, "Bearer"
                )
            );
        }
    }
}
