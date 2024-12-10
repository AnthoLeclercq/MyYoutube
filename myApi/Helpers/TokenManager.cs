using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace myApi.Helpers
{
    public class TokenManager
    {
        public static string GenerateToken(string secret, List<Claim> claims)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler tokenHanlder = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHanlder.CreateToken(tokenDescriptor);

            return tokenHanlder.WriteToken(token);
        }
    }
}
