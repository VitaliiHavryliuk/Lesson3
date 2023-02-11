using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Lesson3API
{
    public static class Utils
    {
        public static bool TryGetEmailFromRequest(HttpRequest req, out string email)
        {
            try
            {
                var token = req.Headers.First(c => c.Key == "Authorization").Value.ToString().Substring("Bearer ".Length);
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                email = securityToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                return true;
            }
            catch
            {
                email = null;
                return false;
            }        
        }
    }
}
